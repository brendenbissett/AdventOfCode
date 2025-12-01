using System.ComponentModel;
using Utilities;

internal class Program
{

    class Part {
        //{x=787,m=2655,a=1222,s=2876}
        public int x { get; set; }
        public int m { get; set; }
        public int a { get; set; }
        public int s { get; set; }

        public Part(int x, int m, int a, int s) {
            this.x = x;
            this.m = m;
            this.a = a;
            this.s = s;
        }
    }

    // "rfg{s<537:gd,x>2440:R,A}"
    class Workflow {
        public string name { get; set; }
        public List<WorkflowStep> steps { get; set; }

        public Workflow(string name, List<WorkflowStep> steps) {
            this.name = name;
            this.steps = steps;
        }

        public void ProcessParts(Dictionary<string, Queue<Part>> results) {

            if(!results.ContainsKey(name)) {
                results.Add(name, new Queue<Part>());
                return;
            }

            while(results[this.name].Count > 0) {
                var part = results[this.name].Dequeue();
                foreach (var step in steps)
                {
                    if(!results.ContainsKey(step.Next)) {
                        results.Add(step.Next, new Queue<Part>());
                    }

                    if(step.Measure == "") {
                        // Just a next step
                        results[step.Next].Enqueue(part);
                        break;
                    }

                    var value = 0;
                    switch(step.Measure) {
                        case "x":
                            value = part.x;
                            break;
                        case "m":
                            value = part.m;
                            break;
                        case "a":
                            value = part.a;
                            break;
                        case "s":
                            value = part.s;
                            break;
                        default:
                            Console.WriteLine($"Unknown Measure: {step.Measure}");
                            break;
                    }

                    var conditionMet = false;

                    switch(step.Operator) {
                        case "<":
                            if(value < step.Value) {
                                conditionMet = true;
                            }
                            break;
                        case ">":
                            if(value > step.Value) {
                                conditionMet = true;
                            }
                            break;

                        case "=":
                            if(value == step.Value) {
                                conditionMet = true;
                            }
                            break;
                        default:
                            Console.WriteLine($"Unknown Operator: {step.Operator}");
                            break;
                    }

                    if(conditionMet) {
                        results[step.Next].Enqueue(part);
                        break;
                    }
                }
            }
        }

    }

    class WorkflowStep {
        public string Measure { get; set; }
        public string Operator { get; set; }
        public int Value { get; set; }
        public string Next { get; set; }
    }

    private static void Main(string[] args)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data/input_day19.txt");
        List<string> lines = FileHandling.ReadInputFile(path);
        Console.WriteLine($"File Input Line Count: {lines.Count}");

        // Part 1
        var (workflows, parts) = ParseInput(lines);
        
        Console.WriteLine($"Workflows: {workflows.Count}");
        Console.WriteLine($"Parts: {parts.Count}");

        ulong part_1 = Solve_Part_1(workflows, parts);

        Console.WriteLine($"Part 1 - Accepted: {part_1}");

        // Tests
        Test_Part_1();
    }

    private static (List<Workflow> workflows, List<Part> parts) ParseInput(List<string> lines)
    {
        List<Workflow> workflows = new List<Workflow>();
        List<Part> parts = new List<Part>();

        bool readingWorkflows = true;
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                readingWorkflows = false;
                continue;
            }

            if (readingWorkflows)
            {
                workflows.Add(GetWorkflow(line));
            }
            else
            {
                parts.Add(GetPart(line));
            }
        }

        return (workflows, parts);
    }

    private static Workflow GetWorkflow(string line) {

        // "rfg{s<537:gd,x>2440:R,A}"

        List<WorkflowStep> workflowSteps = new List<WorkflowStep>();

        var name = line.Substring(0, line.IndexOf('{')); // rfg
        var steps = line.Substring(line.IndexOf('{') + 1, line.IndexOf('}') - line.IndexOf('{') - 1).Split(','); // s<537:gd,x>2440:R,A

        foreach (var step in steps)
        {
            if(!step.Contains(":")) {
                // Just a next step
                workflowSteps.Add(new WorkflowStep() {
                    Measure = "",
                    Operator = "",
                    Value = 0,
                    Next = step
                });
                continue;
            }

            var operator_index = step.IndexOfAny(new char[] { '<', '>', '=' });
            var next_index = step.IndexOf(':');

            var measure = step.Substring(0, operator_index);
            var op = step.Substring(operator_index, 1);
            var value = int.Parse(step.Substring(operator_index + 1, next_index - operator_index - 1));
            var next = step.Substring(next_index + 1);

            workflowSteps.Add(new WorkflowStep() {
                Measure = measure,
                Operator = op,
                Value = value,
                Next = next
            });
        }

        return new Workflow(name, workflowSteps);
    }
    private static Part GetPart(string part) {
        var items = part.Substring(1, part.Length - 2).Split(',');

        int x = 0;
        int m = 0;
        int a = 0;
        int s = 0;

        foreach (var item in items)
        {
            var partlist = item.Split('=');
            var part_prop = partlist[0];
            var part_value = partlist[1];

            switch(part_prop) {
                case "x":
                    x = int.Parse(part_value);
                    break;
                case "m":
                    m = int.Parse(part_value);
                    break;
                case "a":
                    a = int.Parse(part_value);
                    break;
                case "s":
                    s = int.Parse(part_value);
                    break;
            }

        }
        return new Part(x, m, a, s);
    }

    private static ulong Solve_Part_1(List<Workflow> workflows, List<Part> parts)
    {
        Dictionary<string, Workflow> workers = new Dictionary<string, Workflow>();
        foreach (var workflow in workflows)
        {
            workers.Add(workflow.name, workflow);
        }

        Dictionary<string, Queue<Part>> results = new Dictionary<string, Queue<Part>>();

        // Add Accepted and Rejected Dictionary Entries
        results.Add("A", new Queue<Part>());
        results.Add("R", new Queue<Part>());

        // Add the first workflow
        results.Add("in", new Queue<Part>());
        foreach (var part in parts)
        {
            results["in"].Enqueue(part);
        }

        bool processingComplete = false;

        while(!processingComplete) {

            foreach (var workflow in results.Keys.Where(w => w != "R" && w != "A").ToList())
            {
                workers[workflow].ProcessParts(results);
            }

            // See if anything left to process
            ulong count = 0;            
            foreach (var key in results.Keys.Where(w => w != "R" && w != "A").ToList())
            {
                if(results[key].Count > 0) {
                    count += (ulong)results[key].Count;
                }
                else {
                    results.Remove(key);
                }
            }
            
            if(count == 0) {
                processingComplete = true;
            }
        }

        // Add up the accepted parts
        ulong accepted = 0;
        foreach (var part in results["A"])
        {
            accepted += (ulong)part.s + (ulong)part.a + (ulong)part.m + (ulong)part.x;
        }

        return accepted;
    }

    private static void Test_Part_1() {
        List<string> input = new List<string>() {
            "px{a<2006:qkq,m>2090:A,rfg}",
            "pv{a>1716:R,A}",
            "lnx{m>1548:A,A}",
            "rfg{s<537:gd,x>2440:R,A}",
            "qs{s>3448:A,lnx}",
            "qkq{x<1416:A,crn}",
            "crn{x>2662:A,R}",
            "in{s<1351:px,qqz}",
            "qqz{s>2770:qs,m<1801:hdj,R}",
            "gd{a>3333:R,R}",
            "hdj{m>838:A,pv}",
            "",
            "{x=787,m=2655,a=1222,s=2876}",
            "{x=1679,m=44,a=2067,s=496}",
            "{x=2036,m=264,a=79,s=2244}",
            "{x=2461,m=1339,a=466,s=291}",
            "{x=2127,m=1623,a=2188,s=1013}",
        };

        var (workflows, parts) = ParseInput(input);
        
        Console.WriteLine($"Workflows: {workflows.Count}");
        Console.WriteLine($"Parts: {parts.Count}");

        ulong accepted = Solve_Part_1(workflows, parts);

        Console.WriteLine($"Part 1 Testing - Accepted: {accepted}");

    }
}