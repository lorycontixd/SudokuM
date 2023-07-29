using System.Collections.Generic;
using System.IO;
using System;

public sealed class FileX
{
    /**
     * Reads file lines into separate strings stored in List.
     *
     * @param filePath        Full path to the file.
     * @return                If file reading was successful returns
     *                        List<String> containing each lines
     *                        from the file content, otherwise
     *                        returns null. Method do not throws
     *                        IOException.
     */
    public static List<String> readFileLines2ArraList(String filePath)
    {
        List<String> fileContent = null;
        try
        {
            StreamReader sr = new StreamReader(filePath);
            try
            {
                fileContent = new List<String>();
                String line = null;
                while ((line = sr.ReadLine()) != null)
                {
                    fileContent.Add(line);
                }
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
        }
        return fileContent;
    }
    /**
     * Writes the given string into the the given file, previous file
     * content will be overwritten.
     *
     * @param file            File object containing file definition.
     * @param content         String containing content to be written.
     * @return                True if write was successful, otherwise
     *                        returns false. Method do not throws
     *                        IOException.
     */
    public static bool writeFile(String filePath, String content)
    {
        StreamWriter sw;
        try
        {
            sw = new StreamWriter(filePath);
            sw.Write(content);
            sw.Close();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            return false;
        }
    }
    /**
     * Removes file denoted as file path.
     *
     * @param filePath         File pathname.
     * @return                 True if file was removed, false otherwise.
     */
    public static bool removeFile(String filePath)
    {
        if (filePath == null) return false;
        if (filePath.Length == 0) return false;
        if (Directory.Exists(filePath) == true) return false;
        if (File.Exists(filePath) == false) return false;
        FileInfo file = new FileInfo(filePath);
        if (file.IsReadOnly == true) return false;
        try
        {
            file.Delete();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            return false;
        }
    }
    /**
     * Returns temporary directory location.
     * @return Temporary directory location.
     */
    public static String getTmpDir()
    {
        return Path.GetTempPath();
    }
    /**
     * Generates random file name.
     * @param length    File name length (without extension).
     * @param fileExt   File extension.
     * @return          Random file name containing a-zA-Z0-9.
     */
    public static String genRndFileName(int length, String fileExt)
    {
        if (length <= 0) return null;
        if (fileExt == null) return null;
        return randomString(length) + "." + fileExt;
    }
    /**
     * Random string generator.
     * @param length    Length of random string.
     * @return          Random string containing a-zA-Z0-9.
     */
    public static String randomString(int length)
    {
        if (length < 1) return "";
        char[] chars = {
                            'z','x','c','v','b','n','m','a','s','d','f','g','h','j','k','l','q','w','e','r','t','y','u','i','o','p',
                            'Z','X','C','V','B','N','M','A','S','D','F','G','H','J','K','L','Q','W','E','R','T','Y','U','I','O','P',
                            '0','1','2','3','4','5','6','7','8','9'
                };
        String rndStr = "";
        for (int i = 0; i < length; i++)
            rndStr = rndStr + chars[SudokuStore.randomIndex(chars.Length)];
        return rndStr;
    }
}