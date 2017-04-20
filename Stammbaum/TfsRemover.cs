
namespace Stammbaum
{


    public class TfsRemover
    {


        public static void RemoveTFS(string path)
        {
            string input = System.IO.File.ReadAllText(path, System.Text.Encoding.UTF8);
            /*
            input = @"Lorem ipsum dolor sit amet, consectetuer adipiscing elit.
GlobalSection(TeamFoundationVersionControl) = preSolution
    SccNumberOfProjects = 2
    SccEnterpriseProvider = {4CA58AB2-18FA-4F8D-95D4-32DDF27D184C}
    SccTeamFoundationServer = <YourTFSURL>
    SccLocalPath0 = .
    SccProjectUniqueName1 = .
    SccLocalPath1 = .
EndGlobalSection
GlobalSection(nihao) = bar
foo
EndGlobalSection
GlobalSection(TeamFoundationVersionControl) = foo 
bar
EndGlobalSection
GlobalSection(TeamFoundationVersionControl) = braSolution
    SccNumberOfProjects = 2
    SccEnterpriseProvider = {4CA58AB2-18FA-4F8D-95D4-32DDF27D184C}
    SccTeamFoundationServer = <YourTFSURL>
    SccLocalPath0 = .
    SccProjectUniqueName1 = .
    SccLocalPath1 = .
EndGlobalSection

GlobalSection(OMG) = zOMG
ROFL
EndGlobalSection

lorem ipsum";
            */
            string pattern = @"GlobalSection\s*\(\s*TeamFoundationVersionControl\s*\)\s*=\s*.*?\s*EndGlobalSection";
            string output = System.Text.RegularExpressions.Regex.Replace(input, pattern, "",
                System.Text.RegularExpressions.RegexOptions.Singleline
                | System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );
            System.Console.WriteLine(output);
            System.IO.File.WriteAllText(path, output);
        }


        public static void RemoveTFS()
        {
            string basePath = @"C:\Users\username\Desktop\Update-COR-Share";

            if (!System.IO.Directory.Exists(basePath))
                return;

            string[] dirs = System.IO.Directory.GetDirectories(basePath, "*.*", System.IO.SearchOption.AllDirectories);
            foreach (string thisDirectory in dirs)
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(thisDirectory);
                di.Attributes &= ~System.IO.FileAttributes.ReadOnly;
            }


            string[] files = System.IO.Directory.GetFiles(basePath, "*.*", System.IO.SearchOption.AllDirectories);
            foreach (string thisFile in files)
            {
                System.IO.FileInfo di = new System.IO.FileInfo(thisFile);
                di.Attributes &= ~System.IO.FileAttributes.ReadOnly;
            }


            string[] suoFiles = System.IO.Directory.GetFiles(basePath, "*.v12.suo", System.IO.SearchOption.AllDirectories);
            foreach (string thisSuoFile in suoFiles)
            {
                System.IO.File.Delete(thisSuoFile);
            }

            // VBProj = Visual Basic Project file
            // They contain user-specific settings that you have made in the Visual Studio user interface. 
            string[] vbProjUserFiles = System.IO.Directory.GetFiles(basePath, "*.vbproj.user", System.IO.SearchOption.AllDirectories);
            foreach (string thisVbProjUserFiles in vbProjUserFiles)
            {
                System.IO.File.Delete(thisVbProjUserFiles);
            }


            string[] sourceSafeFiles = System.IO.Directory.GetFiles(basePath, "*.vssscc", System.IO.SearchOption.AllDirectories);
            foreach (string thisSourceSafeFile in sourceSafeFiles)
            {
                System.IO.File.Delete(thisSourceSafeFile);
            }

            // VSPSCC = Visual Studio Project Source Code Control
            string[] vbProjSourceSafeFiles = System.IO.Directory.GetFiles(basePath, "*.vspscc", System.IO.SearchOption.AllDirectories);
            foreach (string thisVbProjSourceSafeFiles in vbProjSourceSafeFiles)
            {
                System.IO.File.Delete(thisVbProjSourceSafeFiles);
            }


            string[] solutions = System.IO.Directory.GetFiles(basePath, "*.sln", System.IO.SearchOption.AllDirectories);

            foreach (string thisSolutionPath in solutions)
            {
                RemoveTFS(thisSolutionPath);
            }

            System.Console.WriteLine("Removed TFS & Co.");
        }


    }


}
