using GDF.Data;
using GDF.Data.Static;
using Godot;

namespace Game;

[StaticDataContext("project_context")]
public struct ProjectContext : IDataContext
{
    public bool GetContextVariable(string key, string input, ref Variant output, IDataQueryOptions options)
    {
        switch (key)
        {
            case "version":
            {
                output = "v" + ProjectSettings.GetSetting("application/config/version");
                return true;
            }
        }

        return false;
    }
}