using System;
using BlackBoxDemo;


namespace myProgram
{
    class Program
    {
        public string myData = "This is the public string MyData";
        private string myOtherPrivateData = "This is the PRIVATE string myOtherPrivateData";

        private void myProtectedData()
        {
            Console.WriteLine("This is a PRIVATE method myProtectedData() that prints myOtherPrivateData = " + myOtherPrivateData);
        }
        public void myPublicData()
        {
            Console.WriteLine("This is a public method myPublicData() that prints myData = " + myData);
        }

        static void Main(string[] args)
        {
            BlackBox bb = new BlackBoxDemo.BlackBox();
            Console.WriteLine("Done!");
        }
    }

}