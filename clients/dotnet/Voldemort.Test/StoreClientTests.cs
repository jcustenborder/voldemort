using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Voldemort.Model;
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;

using System.Reflection;

using Microsoft.CSharp;


namespace Voldemort.Test
{
    public class StoreClientTests
    {
        [TestCase]
        public void TestCluster()
        {
            Cluster cluster = new Cluster();
            cluster.Name = "madre";

            string[] servers = new string[] { "wsm-deb2.wsm.local", "wsm-deb3.wsm.local", "wsm-deb4.wsm.local" };
            const int SOCKETPORT = 6666;
            const int HTTPPORT = 8081;
            const int ADMINPORT = SOCKETPORT + 1;



            int index = 0;
            int start = 0;
            foreach (string server in servers)
            {
                int[] partitions = new int[100];
                for (int i = 0; i < partitions.Length; i++)
                    partitions[i] = i + start;

                Node node = new Node();
                node.Host = server;
                node.ID = index++;
                node.SocketPort = SOCKETPORT;
                node.HttpPort = HTTPPORT;
                node.AdminPort = ADMINPORT;
                node.SetPartitions(partitions);
                cluster.Servers.Add(node);
                start += 100;
            }

            using (FileStream iostr = new FileStream("cluster.xml", FileMode.Create, FileAccess.Write))
            {
                Cluster.Save(cluster, iostr);
            }

            


        }

        [Test]
        public void Test()
        {
            string[] Names = Enum.GetNames(typeof(Voldemort.AdminRequestType));

            CodeTypeDeclaration typedeclare = new CodeTypeDeclaration("VoldemortAdminClient");
            typedeclare.IsInterface = true;

            foreach (string Name in Names)
            {
                string[] parts = Name.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < parts.Length; i++)
                    parts[i] = System.Globalization.CultureInfo.CurrentUICulture.TextInfo.ToTitleCase(parts[i].ToLower());
                CodeMemberMethod method = new CodeMemberMethod();
                method.Name = string.Concat(parts);
                typedeclare.Members.Add(method);
            }

            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            CSharpCodeProvider csp = new CSharpCodeProvider();
            csp.GenerateCodeFromType(typedeclare, Console.Out, options);

        }
    }
}
