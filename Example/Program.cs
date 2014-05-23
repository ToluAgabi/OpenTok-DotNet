using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTokSDK;
using OpenTokSDK.Exceptions;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            int apiKey = 44603982; // Replace with your OpenTok API key.
            string apiSecret = "1963c1345671419ea659b0feaa47b1206471463b"; // Replace with your OpenTok API secret.
            string connectionData = "username=Bob,userLevel=4";

            // Creating opentok object to access the opentok API
            OpenTok opentok = new OpenTok(apiKey, apiSecret);

            // Create a session that uses the OpenTok Media Router    
            Session session = opentok.CreateSession();
           
	        // Generate a token from the session we just created	        
            string token = opentok.GenerateToken(session.Id, role: Role.SUBSCRIBER, data: connectionData);

            // We finally print out the id of the session with the new token created
	        Console.Out.WriteLine("SessionId: {0} \ntoken: {1}", session.Id, token);

            
        
        }

        Guid StartArchive(OpenTok opentok, string sessionId, string name)
        {
            try
            {
                Archive archive = opentok.StartArchive(sessionId, name);
                return archive.Id;
            }
            catch (OpenTokException)
            {
                return Guid.Empty;
            }
        }

        bool StopArchive(OpenTok opentok, string archiveId)
        {
            try
            {
                Archive archive = opentok.StopArchive(archiveId);
                return true;
            }
            catch (OpenTokException)
            {
                return false;
            }
        }

        void LogArchiveInfo(OpenTok opentok, string archiveId)
        {
            try
            {
                Archive archive = opentok.GetArchive(archiveId);
                Console.Out.WriteLine("ArchiveId: {0}", archive.Id.ToString());
            }
            catch (OpenTokException exception)
            {
                Console.Out.WriteLine(exception.ToString());
            }
        }

        void ListArchives(OpenTok opentok) 
        {
            try 
            {
    	        ArchiveList archives = opentok.ListArchives();
                for (int i = 0; i < archives.Count(); i++) 
                {
                    Archive archive = archives.ElementAt(i);
                    Console.Out.WriteLine("ArchiveId: {0}", archive.Id.ToString());
                }
            } catch (OpenTokException exception) 
            {
                Console.Out.WriteLine(exception.ToString());
            }
        }
    }
}
