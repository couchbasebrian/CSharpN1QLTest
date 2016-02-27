// CSharpN1QLTest
// Brian Williams
// February 26, 2016

// Connect to a Couchbase 4.x cluster and perform N1QL operations

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Couchbase;
using Couchbase.Core;

// Use NuGet Package Manager to install Couchbase SDK
// PM> install-package CouchbaseNetClient

// Reference material
// http://docs.couchbase.com/sdk-api/couchbase-net-client-2.2.0/
// http://developer.couchbase.com/documentation/server/4.1/sdks/dotnet-2.2/n1ql-queries.html
// http://developer.couchbase.com/documentation/server/4.1/sdks/dotnet-2.2/hello-couchbase.html

// The client currently only supports N1QL when all nodes in the cluster are running it.

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
            String bucketName = "beer-sample";

            TimingClass tc = new TimingClass();
            tc.performTest();
            Console.WriteLine("Time taken was " + tc.getElapsedTime() + " ms.");

            CBConnectTimer cct = new CBConnectTimer("http://127.0.0.1:8091/pools");
            cct.performTest();
            Console.WriteLine("Time taken was " + cct.getElapsedTime() + " ms.");

            CBOpenBucketTimer cbt = new CBOpenBucketTimer(cct.getCluster(), bucketName);
            cbt.performTest();
            Console.WriteLine("Time taken was " + cbt.getElapsedTime() + " ms.");

            CBQueryBucketTimer cqbt = new CBQueryBucketTimer(cbt.getBucket(), bucketName);
            cqbt.performTest();
            Console.WriteLine("Time taken was " + cqbt.getElapsedTime() + " ms.");


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


    class CBQueryBucketTimer : TimingClass
    {
        IBucket bucket;
        String bucketName;

        public CBQueryBucketTimer(IBucket b, String name)
        {
            bucket = b;
            bucketName = name;
        }

        public override void doTheWork()
        {
            String queryAll = "SELECT * FROM `" + bucketName + "`";

            Console.WriteLine("About to query: " + queryAll);

            var result = bucket.Query<dynamic>(queryAll);

            Console.WriteLine("After the query...");

            var errors = result.Errors;
            var success = result.Success;
            Console.WriteLine("Errors:  " + errors);
            Console.WriteLine("Success: " + success);
            foreach (var row in result.Rows)
            {
                Console.WriteLine(row);
            }
        }
    }

    class CBOpenBucketTimer : TimingClass
    {

        Couchbase.Cluster myCluster;
        Couchbase.Core.IBucket bucket;
        String bucketName;

        // Given a cluster and a bucket name, open the bucket
        public CBOpenBucketTimer(Couchbase.Cluster c, String bName)
        {
            myCluster = c;
            bucketName = bName;
        }

        public Couchbase.Core.IBucket getBucket() { return bucket; }

        public override void doTheWork()
        {
            Console.WriteLine("About to open the bucket " + bucketName);
            bucket = myCluster.OpenBucket(bucketName);
        }

    } // Open a bucket



    class CBConnectTimer : TimingClass
    {
        Couchbase.Cluster sourceCluster;
        String hostName;

        public CBConnectTimer(String s)
        {
            hostName = s;
        }

        public Couchbase.Cluster getCluster() { return sourceCluster; }

        public override void doTheWork()
        {
            Console.WriteLine("About to connect to cluster at " + hostName);

            var config = new Couchbase.Configuration.Client.ClientConfiguration
            {
                Servers = new List<Uri>
                    {
                        new Uri(hostName)
                    }
            };

            sourceCluster = new Couchbase.Cluster(config);
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
        public virtual void doTheWork()
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
                Console.WriteLine("An exception occurred!" + e);
                caughtException = e;
                exceptionOccurred = true;
            }
            stopTiming();
        }

    } // TimingClass

} // CSharpN1QLTest namespace




// EOF