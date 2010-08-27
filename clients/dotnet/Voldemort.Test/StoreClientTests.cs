using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
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

            CodeTypeDeclaration implementation = new CodeTypeDeclaration("VoldemortAdminClient");
            implementation.IsClass = true;
            implementation.Attributes = MemberAttributes.Public;
            CodeTypeDeclaration interfaceType = new CodeTypeDeclaration("IVoldemortAdminClient");
            interfaceType.IsInterface = true;
            interfaceType.Attributes = MemberAttributes.Public;

            foreach (string Name in Names)
            {
                string[] parts = Name.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < parts.Length; i++)
                    parts[i] = System.Globalization.CultureInfo.CurrentUICulture.TextInfo.ToTitleCase(parts[i].ToLower());
                CodeMemberMethod method = new CodeMemberMethod();
                method.Name = string.Concat(parts);
                interfaceType.Members.Add(method);

                string typeName = string.Format("Voldemort.{0}Request, Voldemort", method.Name);


                Type type = Type.GetType(typeName);

                if (null == type)
                    continue;

                PropertyInfo[] properties = type.GetProperties();

                foreach (PropertyInfo property in properties)
                {
                    CodeParameterDeclarationExpression methodParameter = new CodeParameterDeclarationExpression(property.PropertyType, property.Name);
                    method.Parameters.Add(methodParameter);
                }
            }

            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            options.BlankLinesBetweenMembers = false;

            CSharpCodeProvider csp = new CSharpCodeProvider();
            csp.GenerateCodeFromType(interfaceType, Console.Out, options);
        }
    }
}
