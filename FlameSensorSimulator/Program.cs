using System;
using System.Collections.Generic;
using System.Threading;

namespace Oleg
{
    internal class Program
    {
        enum SensorState
        {
            NO_FLAME,
            FLAME_DETECTED,
            SENSOR_ERROR,
            USED_BUTTON,
            DISABLED
        }

        enum SignalType
        {
            FLAME,
            BUTTON,
            ERROR,
            NONE
        }

        enum InvokeType
        {
            FLAME,
            BUTTON,
            ERROR,
            DISABLE
        }

        class Sensor
        {
            public int ID;
            public SensorState State;
            public Sensor(int SensorID)
            {
                this.ID = SensorID;
                this.State = SensorState.NO_FLAME;
            }
        }

        static string SensorStatetoString(SensorState state)
        {
            switch (state)
            {
                case SensorState.NO_FLAME:
                    return "OK";
                case SensorState.FLAME_DETECTED:
                    return "FD";
                case SensorState.USED_BUTTON:
                    return "UB";
                case SensorState.SENSOR_ERROR:
                    return "SE";
                case SensorState.DISABLED:
                    return "DS";
                default:
                    return "??"; // It`s possible?
            }
        }

        static void setF(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }

        static SignalType signalType = SignalType.NONE;
        static InvokeType invokeType = InvokeType.FLAME;

        static void signal()
        {
            while (true)
            {
                switch (signalType)
                {
                    case SignalType.FLAME:
                        Console.Beep(500, 500);
                        Console.Beep(500, 500);
                        Console.Beep(1000, 1000);
                        break;
                    case SignalType.BUTTON:
                        Console.Beep(500, 500);
                        Console.Beep(500, 500);
                        Console.Beep(500, 500);
                        Console.Beep(1100, 500);
                        break;
                    case SignalType.ERROR:
                        Console.Beep(1100, 500);
                        Console.Beep(1100, 500);
                        Console.Beep(1100, 500);
                        Console.Beep(1100, 500);
                        break;
                }
            }
        }


        static void Main(string[] args)
        {
            bool isFlame = false;
            Thread thread = new Thread(signal);
            thread.Start();
            List<Sensor> sensors = new List<Sensor>();
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Fire alarm");
                if (sensors.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("No connected sensors");
                    Console.WriteLine("To connect sensor, press Q");
                }
                else
                {
                    setF(ConsoleColor.Blue);
                    string sc = "0";
                    foreach (Sensor sensor in sensors)
                    {
                        Console.Write("S" + sc + " ");
                        sc = (Convert.ToInt32(sc) + 1).ToString();
                    }
                    Console.Write('\n');
                    foreach (Sensor sensor in sensors)
                    {
                        switch (sensor.State)
                        {
                            case SensorState.NO_FLAME:
                                setF(ConsoleColor.Green);
                                break;
                            case SensorState.FLAME_DETECTED:
                                setF(ConsoleColor.Red);
                                break;
                            case SensorState.USED_BUTTON:
                                setF(ConsoleColor.DarkRed);
                                break;
                            case SensorState.SENSOR_ERROR:
                                setF(ConsoleColor.Yellow);
                                break;
                            case SensorState.DISABLED:
                                setF(ConsoleColor.Gray);
                                break;
                            default:
                                setF(ConsoleColor.White);
                                break;
                        }
                        Console.Write(SensorStatetoString(sensor.State) + " ");
                    }
                    Console.Write("\n\n");
                    setF(ConsoleColor.DarkRed);
                    Console.WriteLine("Invoke type: " + invokeType.ToString());
                }
                ConsoleKeyInfo resultKey = Console.ReadKey();
                ConsoleKey key = resultKey.Key;
                if (key == ConsoleKey.Q)
                {
                    if (sensors.Count != 10)
                    {
                        sensors.Add(new Sensor(sensors.Count));
                    }
                }
                if (key == ConsoleKey.E)
                {
                    try
                    {
                        sensors.RemoveAt(sensors.Count - 1);
                    }
                    catch (Exception)
                    {
                        // nothing =)
                    }
                }
                if (key == ConsoleKey.X)
                {
                    switch (invokeType)
                    {
                        case InvokeType.FLAME:
                            invokeType = InvokeType.BUTTON;
                            break;
                        case InvokeType.BUTTON:
                            invokeType = InvokeType.ERROR;
                            break;
                        case InvokeType.ERROR:
                            invokeType = InvokeType.DISABLE;
                            break;
                        case InvokeType.DISABLE:
                            invokeType = InvokeType.FLAME;
                            break;
                    }
                }
                foreach (Sensor sensor in sensors)
                {
                    ConsoleKey kPress = (ConsoleKey)Enum.Parse(typeof(ConsoleKey), "D" + sensor.ID.ToString());
                    if (key == kPress)
                    {
                        if (sensor.State == SensorState.NO_FLAME)
                        {
                            switch (invokeType)
                            {
                                case InvokeType.FLAME:
                                    sensor.State = SensorState.FLAME_DETECTED;
                                    signalType = SignalType.FLAME;
                                    break;
                                case InvokeType.BUTTON:
                                    sensor.State = SensorState.USED_BUTTON;
                                    signalType = SignalType.BUTTON;
                                    break;
                                case InvokeType.ERROR:
                                    sensor.State = SensorState.SENSOR_ERROR;
                                    signalType = SignalType.ERROR;
                                    break;
                                case InvokeType.DISABLE:
                                    sensor.State = SensorState.DISABLED;
                                    signalType = SignalType.NONE;
                                    break;
                            }
                        }
                        else
                        {
                            sensor.State = SensorState.NO_FLAME;
                        }
                    }
                }
                foreach (Sensor sensor in sensors)
                {
                    if (sensor.State != SensorState.NO_FLAME)
                    {
                        isFlame = true;
                        break;
                    }
                    else
                    {
                        isFlame = false;
                    }
                }
                if (!isFlame)
                {
                    signalType = SignalType.NONE;
                }
            }
        }
    }
}