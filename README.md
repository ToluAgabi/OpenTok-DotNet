# OpenTok .NET SDK 2.2

Use the OpenTok .NET SDK to work with [OpenTok](http://www.tokbox.com/) applications.
You can create OpenTok [sessions](http://tokbox.com/opentok/tutorials/create-session/)
and to generate [tokens](http://tokbox.com/opentok/tutorials/create-token/),
and work with OpenTok 2.0 [archives](http://tokbox.com/#archiving).

## Download

Download the .NET SDK:

<https://github.com/opentok/Opentok-DotNet/archive/master.zip>

# Documentation

Reference documentation is available at <http://www.tokbox.com//opentok/libraries/server/dot-net/reference/index.html> and in the
docs directory of the SDK.

# Creating Sessions
Use the `createSession()` method of the OpenTok object to create a session and a session ID.

The following code creates a session that uses the OpenTok Media Router:

<pre>
using OpenTokSDK;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
	        int apiKey = 0; // Replace with your OpenTok API key.
	        string apiSecret = ""; // Replace with your OpenTok API secret.

	        OpenTok opentok = new OpenTok(apiKey, apiSecret);
	        Session session = opentok.CreateSession();
	        
	        //Generate a session that uses the OpenTok Media Router    
	        Console.Out.WriteLine("SessionId: %s", session.Id);
        }
    }
}
</pre>

The following code creates a peer-to-peer session:

<pre>
using OpenTokSDK;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
	        int apiKey = 0; // Replace with your OpenTok API key.
	        string apiSecret = ""; // Replace with your OpenTok API secret.

	        OpenTok opentok = new OpenTok(apiKey, apiSecret);
	        Session session = opentok.CreateSession(mediaMode = MediaMode.RELAY);
	        
	        //Generate a session that uses the OpenTok Media Router    
	        Console.Out.WriteLine("SessionId: %s", session.Id);
        }
    }
}
</pre>

# Generating tokens
Use the  `generateTokentoken()` method of the OpenTokSDK object to create an OpenTok token:

The following example shows how to obtain a token:

<pre>
using OpenTokSDK;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
	        int apiKey = 0; // Replace with your OpenTok API key.
	        string apiSecret = ""; // Replace with your OpenTok API secret.

	        OpenTok opentok = new OpenTok(apiKey, apiSecret);
	         //Generate a basic session. Or you could use an existing session ID.
	        Session session = opentok.CreateSession(mediaMode = MediaMode.RELAY);

	        string token = opentok.GenerateToken(session.Id)

	        Console.Out.WriteLine("SessionId: %s \ntoken: %s", session.Id, token);
        }
    }
}
</pre>

The following C# code example shows how to obtain a token that has a role of "subscriber" and that has
a connection metadata string:

<pre>
using OpenTokSDK;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
	        int apiKey = 0; // Replace with your OpenTok API key.
	        string apiSecret = ""; // Replace with your OpenTok API secret.

	        OpenTok opentok = new OpenTok(apiKey, apiSecret);
	        Session session = opentok.CreateSession(mediaMode = MediaMode.RELAY);
	        string connectionData = "username=Bob,userLevel=4";

	        string token = opentok.Generate(session.Id, role=Role.SUBSCRIBER, data=connectionData);

	        //Generate a session that uses the OpenTok Media Router    
	        Console.Out.WriteLine("SessionId: %s \ntoken: %s", session.Id, token);
        }
    }
}
</pre>

# Working with OpenTok 2.0 archives

The following method starts recording an archive of an OpenTok 2.0 session (given a session ID)
and returns the archive ID (on success).

<pre>
Guid StartArchive(OpenTok opentok, string sessionId, string name) {
    try {
        Archive archive = opentok.startArchive(sessionId, name);
        return archive.Id;
    } catch (OpenTokException exception){
        return null;
    }
}
</pre>

The following method stops the recording of an archive (given an archive ID), returning
true on success, and false on failure.

<pre>
bool stopArchive(OpenTok opentok, string archiveId) {
    try {
        Archive archive = opentok.StopArchive(archiveId);
        return true;
    } catch (OpenTokException exception){
        return false;
    }
}
</pre>

The following method logs information on a given archive.

<pre>
void LogArchiveInfo(OpenTok opentok, string archiveId) {
    try {
        Archive archive = opentok.GetArchive(archiveId);
        Console.Out.WriteLine(archive.ToString());
    } catch (OpenTokException exception){
        Console.Out.WriteLine(exception.ToString());
    }
}
</pre>

The following method logs information on all archives (up to 50)
for your API key:

<pre>
void ListArchives(OpenTok opentok) {
    try {
    	ArchiveList archives = opentok.ListArchives();
        for (int i = 0; i &lt; archives.Count(); i++) {
            Archive archive = archives.get(i);
            Console.Out.WriteLine(archive.ToString());
        }
    } catch (OpenTokException exception) {
        Console.Out.WriteLine(exception.ToString());
    }
}
</pre>


# Support

See http://tokbox.com/opentok/support/ for all our support options.

Find a bug? File it on the [Issues](https://github.com/opentok/opentok-php-sdk/issues) page. Hint:
test cases are really helpful!
