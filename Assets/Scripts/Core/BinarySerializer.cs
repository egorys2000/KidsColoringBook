using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using UnityEngine;

public static class BinarySerializer
{
    public static T DeepClone<T>(this T obj)
    {
        using (var ms = new MemoryStream())
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(ms, obj);
            ms.Position = 0;

            return (T)formatter.Deserialize(ms);
        }
    }

    public static void Serialize<T>(T dataToSerialize, string fileName)
    {
        FileStream fs;

        if (!File.Exists(fileName))
            fs = new FileStream(fileName, FileMode.Create);
        else
            fs = new FileStream(fileName, FileMode.Create);

        BinaryFormatter formatter = new BinaryFormatter();
        try
        {
            formatter.Serialize(fs, dataToSerialize);
        }
        catch (SerializationException e)
        {
            Debug.LogError("Failed to serialize. Reason: " + e.Message);
            throw;
        }
        finally
        {
            fs.Close();
        }
    }

    public static T Deserialize<T>(string fileName)
    {
        FileStream fs;

        if (File.Exists(fileName))
            fs = new FileStream(fileName, FileMode.Open);
        else
            return default(T);

        BinaryFormatter formatter = new BinaryFormatter();
        T toReturn = default(T);
        try
        {
            toReturn = (T)formatter.Deserialize(fs);
        }
        catch (SerializationException e)
        {
            Debug.LogError("Failed to serialize. Reason: " + e.Message);
            throw;
        }
        finally
        {
            fs.Close();
        }

        return toReturn;
    }
}
