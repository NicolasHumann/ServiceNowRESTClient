﻿using System;
using ServiceNow;
using ServiceNow.TableAPI;
using ServiceNow.TableDefinitions;

namespace UsageDemo
{
    class Program
    {
        static string myInstance = "instanceName";
        static string instanceUserName = "user";
        static string instancePassword = "pass";

        static void Main(string[] args)
        {
            // Shows basic CRUD operations with a simple small subset of a record
            //basicOperations();

            // Demonstrates a more advanced retrieval including related resource by link and dot traversal.
            retrieveByQuery();

            // Break
            Console.ReadLine();
        }

        static void basicOperations()
        {
            TableAPIClient<sys_user> client = new TableAPIClient<sys_user>("sys_user", myInstance, instanceUserName, instancePassword);

            // Create a user
            var createdUser = client.Post(new sys_user()
            {
                first_name = "Tester",
                last_name = "McTester",
                u_employee_number = "999888",
                u_unavailable = false,
                u_location = "",
                u_date_available = ""
            });
            Console.WriteLine("User Created: " + createdUser.result.first_name + " " + createdUser.result.last_name + " (" + createdUser.result.sys_id + ")");


            // Retrieve the user (even though we already have it
            var retrievedUser = client.GetById(createdUser.result.sys_id);
            Console.WriteLine("User Retrieved: " + retrievedUser.result.first_name + " " + retrievedUser.result.last_name + " (" + retrievedUser.result.sys_id + ")");


            // Update the User
            if (retrievedUser.result != null)
            {
                var d = retrievedUser.result;
                d.u_unavailable = true;
                d.u_location = "somewhere else";
                d.u_date_available = DateTime.Now.AddHours(3).ToString();

                var updatedUser = client.Put(d);
                Console.WriteLine("Updated Location: " + updatedUser.result.u_location);
            }
            

            // Delete the user
            client.Delete(retrievedUser.result.sys_id);
        }

        static void retrieveByQuery()
        {
            var query = "active=true^u_resolved=false";
            TableAPIClient<incident> client = new TableAPIClient<incident>("incident", myInstance, instanceUserName, instancePassword);

            foreach (incident r in client.GetByQuery(query).result)
            {
                DateTime openedOn = DateTime.Parse(r.opened_at);
                ResourceLink openedFor = r.caller_id;

                Console.WriteLine(r.number + " :  " + r.short_description + " (Opened " + openedOn.ToShortDateString() + " for " + r.caller_first_name + ")");
            }
        }
    }
}