// CSharpN1QLTest
// Brian Williams
// February 26, 2016

// Connect to a Couchbase 4.x cluster and perform N1QL operations

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Use NuGet Package Manager to install Couchbase SDK
// PM> install-package CouchbaseNetClient

// Reference material
// http://developer.couchbase.com/documentation/server/4.1/sdks/dotnet-2.2/n1ql-queries.html
// http://developer.couchbase.com/documentation/server/4.1/sdks/dotnet-2.2/hello-couchbase.html


namespace CSharpN1QLTest
{
    class CSharpN1QLTest
    {
        static void Main(string[] args)
        {
            CSharpN1QLTest csnt = new CSharpN1QLTest();

            csnt.runTests();
        }

        private void runTests()
        {
            TimingClass tc = new TimingClass();
            tc.performTest();
            Console.WriteLine("Time taken was " + tc.getElapsedTime() + " ms.");
        }
    } // class CSharpN1QLTest

    // This helper class comes from
    // http://stackoverflow.com/questions/290227/java-system-currenttimemillis-equivalent-in-c-sharp
    //
    public static class DateTimeExtensions
    {
        private static DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static long currentTimeMillis()
        {
            return (long)((DateTime.UtcNow - Jan1st1970).TotalMilliseconds);
        }
    }

    class CBConnectTimer : TimingClass
    {
        Couchbase.Cluster sourceCluster;
        String hostName;

        public CBConnectTimer(String s)
        {
            hostName = s;
        }

        public Couchbase.Cluster getCluster() { return sourceCluster; }

        public void doTheWork()
        {
            sourceCluster = new Couchbase.Cluster(hostName);
        }

    }

    class TimingClass
    {
        long startTime, endTime;
        bool exceptionOccurred;
        Exception caughtException;

        public TimingClass()
        {
            startTime = 0;
            endTime = 0;
            caughtException = null;
            exceptionOccurred = false;
        }

        public void startTiming() { startTime = DateTimeExtensions.currentTimeMillis(); }
        public void stopTiming() { endTime = DateTimeExtensions.currentTimeMillis(); }

        public long getElapsedTime()
        {
            return (endTime - startTime);
        }

        public bool didExceptionOccur() { return exceptionOccurred; }

        public Exception getException() { return caughtException; }

        // override in subclass
        public void doTheWork()
        {
            Console.WriteLine("This is where you do something");
        }

        public void performTest()
        {

           Console.WriteLine("I am " + this.GetType().Name);

            // call the method that can be overridden
            startTiming();
            try
            {
                doTheWork();
            }
            catch (Exception e)
            {
                caughtException = e;
                exceptionOccurred = true;
            }
            stopTiming();
        }

    } // TimingClass

} // CSharpN1QLTest namespace




// EOF