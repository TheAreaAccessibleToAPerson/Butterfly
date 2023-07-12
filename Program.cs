using System.ComponentModel;
namespace Butterfly
{
    public class Program
    {
        static void Main(string[] args)
        {
            Butterfly.fly<Header2>(new Butterfly.Settings()
            {
                Name = "",
                StartingTime = 1000,
                SystemEvent = new EventSetting("SYSTEM", 10, 256000),

                Events = new EventSetting[]
               {
                    new EventSetting("1", 5, 356000),
                    new EventSetting("2", 5, 3256000),
                    new EventSetting("3", 50, 3256000),
                    new EventSetting("4", 50, 3256000)
               }
            });
        }
    }

    public sealed class Header : Controller
    {
        void Start()
        {
            obj<Header2>("HEADER2");
        }
    }


    public sealed class Header2 : Controller
    {
        void Construction()
        {
            listen_echo<string>("echotest")
                .output_to((message, response) =>
                {
                    Console(message);

                    response.To("ответ");
                });

            listen_message<string>("messagetest")
                .output_to((message) =>
                {
                    Console(message);
                });

            add_event("1", () =>
            {
                if (i < 10)
                {
                    obj<Test2>(i++.ToString());
                }
            });


            //add_event("1", Update);
            //obj<Test2>("TEST" + 88);
        }

        int i = 0;
        void Update()
        {
        }

        void Start()
        {
        }

        void Configurate()
        {
        }

        void Stop()
        {
            Console("STOP!!");
        }
    }

    public sealed class Test2 : Controller
    {
        IInput<string> send_echo, send_message;

        void Construction()
        {
            send_echo<string>(ref send_echo, "echotest")
                .output_to((message) =>
                {
                });

            send_message<string>(ref send_message, "messagetest");

            //obj<Test3>("TEST3");
            /*
            obj<Test3>("TEST31");
            obj<Test3>("TEST32");
            obj<Test3>("TEST33");
            obj<Test3>("TEST34");
            obj<Test3>("TEST35");
            obj<Test3>("TEST36");
            obj<Test3>("TEST37");
            obj<Test3>("TEST37");
            */

            add_event("1", Update);
        }

        int i = 0;
        void Update()
        {
            if (i < 100)
            {
                obj<Test3>(i++.ToString());

                if (i == 88)
                    destroy();

                //obj<Test4>(i++.ToString());
                //obj<Test5>(i++.ToString());
            }
        }

        void Start()
        {
            //obj<Test3>("TEST37");
            send_echo.To(GetKey());
            send_message.To("Hello2");
        }

        void Stop()
        {
        }

        void Configurate()
        {
        }
    }

    public sealed class Test3 : Controller
    {
        public static int i = 0;
        void Construction()
        {
            obj<Test4>(i++.ToString());
            obj<Test4>(i++.ToString());
            obj<Test4>(i++.ToString());
            obj<Test4>(i++.ToString());
            obj<Test4>(i++.ToString());
            obj<Test4>(i++.ToString());
            obj<Test4>(i++.ToString());
            obj<Test4>(i++.ToString());
            obj<Test4>(i++.ToString());
            obj<Test4>(i++.ToString());
            obj<Test4>(i++.ToString());
            obj<Test4>(i++.ToString());
            obj<Test4>(i++.ToString());
            obj<Test4>(i++.ToString());
            obj<Test4>(i++.ToString());
            obj<Test4>(i++.ToString());
            obj<Test4>(i++.ToString());
            obj<Test4>(i++.ToString());
        }

        void Update()
        {
        }

        void Start()
        {
            if (GetKey() == "88")
            {
                destroy();
            }
        }

        void Configurate()
        {
        }
    }

    public sealed class Test4 : Controller
    {
        void Construction()
        {
            //destroy();
            add_event("4", Update1);
            add_event("3", Update1);

            obj<Test5>("AAAA");
            obj<Test5>("21111");
            obj<Test5>("31111");
            obj<Test5>("41111");
            obj<Test5>("51111");
        }

        void Update1()
        {
            /*
            if (GetKey() == "77")
            {
                destroy();
            }
            */
        }
        int i = 0;

        void Start()
        {
            //Console("START");
        }

        void Configurate()
        {
        }
    }

    public sealed class Test5 : Controller
    {
        void Construction()
        {
            add_event("1", Update1);
            add_event("2", Update2);
            add_event("3", Update3);
            add_event("4", Update1);

            /*
                        listen_message<int>("message_test2")
                            .output_to((message) => 
                            {
                                destroy();
                            });
                            */
        }

        static int i = 0;
        void Update1()
        {
            if (i < 10)
                obj<Test6>(i++.ToString());
        }
        void Update2()
        {
            //obj<Test6>(i++.ToString());
        }
        void Update3()
        {
            //obj<Test6>(i++.ToString());
        }

        int u = 0;
        void Update4()
        {
            //obj<Test6>(i++.ToString());
        }
        void Start()
        {
            //Console("STart");
        }

        void Configurate()
        {
        }
    }

    public sealed class Test6 : Controller
    {
        IInput<int> _input;

        void Construction()
        {
            //send_message<int>(ref _input, "message_test2");

            add_event("1", Update);
        }

        void Start()
        {
        }

        void Update()
        {
            /*
            if (GetKey() == "77")
            {
                destroy();
            }
            */
        }

        void Stop()
        {
            // _input.To(1);
        }
    }
}
