using Superpower.Parsers;
using Utilities;

internal class Program
{

    interface IModule {

        public string Id { get; set; }

        public bool IsOn { get; set; }

        public List<IModule> destinations { get; set; }

        public Dictionary<string, int> inputs { get; set; }

        void AddDestination(IModule destination);

        public void AddInput(string id);

        public List<string> SendPulse(string source, int pulse, Dictionary<string, IModule> modules); // Return "ID,Pulse" for what this module sends

    }
    class FlipFlop : IModule
    {
        public string Id { get; set; }
        public bool IsOn { get; set; }
        public Queue<int> input { get; set; }
        public List<IModule> destinations { get; set; }
        public Dictionary<string, int> inputs { get; set; }

        public FlipFlop(string id)
        {
            Id = id;
            this.IsOn = false;

            this.input = new Queue<int>();
            this.inputs = new Dictionary<string, int>();  // Input ID, Last Pulse
            this.destinations = new List<IModule>();    
        }

        public List<string> SendPulse(string sourceId, int pulse, Dictionary<string, IModule> modules)
        {
            List<string> pulses = new List<string>();

            if(pulse == 1) return pulses;  // Ignore high pulses

            int pulseToSend = (!this.IsOn) ? 1 : 0;
            this.IsOn = !this.IsOn;

            foreach(IModule destination in this.destinations) {
                pulses.Add($"{this.Id}->{destination.Id},{pulseToSend}");
            }
            
            return pulses;
        }
  
        public void AddDestination(IModule destination)
        {
            this.destinations.Add(destination);
        }

        public void AddInput(string id)
        {
            this.inputs[id] = 0;
        }
    }

    class Conjunction : IModule
    {
        public string Id { get; set; }
        public Dictionary<string, int> inputs { get; set; }
        public List<IModule> destinations { get; set; }
        public bool IsOn { get; set; }

        public Conjunction(string id)
        {
            Id = id;

            this.IsOn = false;
            this.inputs = new Dictionary<string, int>();  // Input ID, Last Pulse
            this.destinations = new List<IModule>();    
        }

        public void AddInput(string id)
        {
            this.inputs[id] = 0;
        }

        public void AddDestination(IModule destination)
        {
            this.destinations.Add(destination);
        }

        public List<string> SendPulse(string sourceId, int pulse, Dictionary<string, IModule> modules)
        {
            bool allHigh = true;

            this.inputs[sourceId] = pulse; 

            if (this.inputs.Values.Sum() == this.inputs.Count) {
                allHigh = true;
            } else {
                allHigh = false;
            }

            int pulseToSend = allHigh ? 0 : 1;
            
            List<string> pulses = new List<string>();

            foreach(IModule destination in this.destinations) {
                pulses.Add($"{this.Id}->{destination.Id},{pulseToSend}");
            }

            return pulses;
        }
    }

    class Broadcaster : IModule {
        public string Id { get; set; }

        public List<IModule> destinations { get; set; }

        public bool IsOn { get; set; }
        public Dictionary<string, int> inputs { get; set; }

        public Broadcaster(string id)
        {
            Id = id;
            this.IsOn = false;
            this.inputs = new Dictionary<string, int>();  // Input ID, Last Pulse
            this.destinations = new List<IModule>();    
        }

        public void AddDestination(IModule destination)
        {
            this.destinations.Add(destination);
        }

        public void AddInput(string id)
        {
            this.inputs[id] = 0;
        }

        public List<string> SendPulse(string id, int pulse, Dictionary<string, IModule> modules)
        {
            List<string> pulses = new List<string>();

            foreach(IModule destination in this.destinations) {
                pulses.Add($"{this.Id}->{destination.Id},{pulse}");
            }

            return pulses;
        }
    }

    class GenericModule : IModule
    {
        public string Id { get; set; }
        public bool IsOn { get; set;}
        public List<IModule> destinations { get; set; }
        public Dictionary<string, int> inputs { get; set; } 

        public GenericModule(string id)
        {
            this.Id = id;
            this.IsOn = false;
            this.destinations = new List<IModule>();
            this.inputs = new Dictionary<string, int>();
        }
        public void AddDestination(IModule destination)
        {
            this.destinations.Add(destination);
        }

        public void AddInput(string id)
        {
            this.inputs[id] = 0;
        }

        public List<string> SendPulse(string source, int pulse, Dictionary<string, IModule> modules)
        {
            // Do nothing
            return new List<string>();
        }
    }

    private static void Main(string[] args)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data/input_day20.txt");
        List<string> lines = FileHandling.ReadInputFile(path);
        Console.WriteLine($"File Input Line Count: {lines.Count}");

        // Part 1
        Console.WriteLine("Part 1 - START ----------");
        Solve_Part_1(lines, 1000, false); 
        // 3590163 -- Too Low
        // 6000000 -- Too Low
        // 9000000 -- Too Low (Changed the input file)
        // 730797576  -- Correct

        Console.WriteLine("Part 1 - END ----------");

        // Tests
        Part_1_Test();
        Part_1_Test_2();
    }

    private static void Solve_Part_1(List<string> input, ulong pushCount, bool debug) {

        var modules = GetModules(input);

        ulong lowPulsesSent = 0;
        ulong highPulsesSent = 0;
        Queue<string> pulses = new Queue<string>();

        for(ulong i=0; i<pushCount; i++) 
        {

            // Send pulse(s) to the broadcaster
            lowPulsesSent++;
            if(debug) {
                Console.WriteLine($"button -> broadcaster, 0");
            }

            foreach(var item in modules["broadcaster"].SendPulse("aptly", 0, modules)) {
                pulses.Enqueue(item);
            }

            while(pulses.Count > 0) {

                string pulse = pulses.Dequeue();
                string source = pulse.Split("->")[0];
                string id = pulse.Split("->")[1].Split(",")[0];
                int value = int.Parse(pulse.Split("->")[1].Split(",")[1]);

                if(debug) {
                    Console.WriteLine($"Sending Pulse: {pulse}");
                }

                if(value == 0) {
                    lowPulsesSent++;
                } else {
                    highPulsesSent++;
                }

                if(!modules.ContainsKey(id)) {
                    Console.WriteLine($"Solve_Part_1 - Module {id} does not exist");
                    continue;
                }
                List<string> newPulses = modules[id].SendPulse(source, value, modules);
                foreach(string newPulse in newPulses) {
                    pulses.Enqueue(newPulse);
                }
            }

        }

        ulong total = lowPulsesSent * highPulsesSent;

        Console.WriteLine($"Pulses Sent: {total} (Low: {lowPulsesSent}, High: {highPulsesSent})");
    }

    private static Dictionary<string, IModule> GetModules(List<string> input) {
        Dictionary<string, IModule> modules = new Dictionary<string, IModule>();

        // Create each module
        foreach(string line in input) {
            string moduleInfo = line.Split("->")[0].Trim();

            char type = moduleInfo[0];
            string id = moduleInfo.Substring(1);

            switch(type) {
                case '%':
                    modules[id] = new FlipFlop(id);
                    break;
                case '&':
                    modules[id] = new Conjunction(id);
                    break;
                case 'b':
                    modules["broadcaster"] = new Broadcaster("broadcaster");
                    break;
                default:                    
                    break;
            }
        }

        // Connect the modules (Inputs and Destinations)
        foreach(string line in input) {
            string id = line.Split("->")[0].Trim().Substring(1);
            string[] destinations = line.Split("->")[1].Trim().Replace(" ", "").Split(",");

            if (id == "roadcaster") {
                id = "broadcaster";
            }

            foreach(string destination in destinations) {
                if(!modules.ContainsKey(destination)) {
                    Console.WriteLine($"GetModules - Module {destination} does not exist. Adding Destination anyway.");
                    modules[destination] = new GenericModule(destination);
                }
                modules[id].AddDestination(modules[destination.Trim()]);
                modules[destination].AddInput(id);
            }
        }

        return modules;
    }

    private static void Part_1_Test() {
        List<string> input = new List<string>() {
            "broadcaster -> a, b, c",
            "%a -> b",
            "%b -> c",
            "%c -> inv",
            "&inv -> a"
        };

        Console.WriteLine();
        Console.WriteLine("Part 1 Test 1 - START");
        Solve_Part_1(input, 1000, false);
        Console.WriteLine("Part 1 Test 1 - END");
        Console.WriteLine();
    }

    private static void Part_1_Test_2() {
        List<string> input = new List<string>() {
            "broadcaster -> a",
            "%a -> inv, con",
            "&inv -> b",
            "%b -> con",
            "&con -> output",
        };

        Console.WriteLine();
        Console.WriteLine("Part 1 Test 2 - START");
        Solve_Part_1(input, 1000, false);  // 11687500
        Console.WriteLine("Part 1 Test 2 - END");
        Console.WriteLine();
    }
}