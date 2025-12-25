using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class NetworkLayerInstaller
{
    private const string LAYER_PLAYER = "Player";
    private const string LAYER_BOUNDS = "CameraBounds";

    private static string GetProjectKey() => 
        $"NGO_Steam_Transporter_{Application.dataPath.GetHashCode()}";
    static NetworkLayerInstaller()
    {
        if (EditorPrefs.GetBool(GetProjectKey(), false))
        {
            return; 
        }
        
        RunSetup();
    }
    
    [MenuItem("Tools/NGO-Steam-Transporter/Fix Layers & Physics")]
    public static void ForceRunSetup()
    {
        RunSetup();
        Debug.Log("[NGO-Steam-Transporter] Ayarlar manuel olarak güncellendi.");
    }

    private static void RunSetup()
    {
        UpdateLayers();
        
        SetupPhysicsMatrix();

        EditorPrefs.SetBool(GetProjectKey(), true);
    }
    
    private static void UpdateLayers()
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layersProp = tagManager.FindProperty("layers");

        bool layerAdded = false;
        string[] requiredLayers = { LAYER_PLAYER, LAYER_BOUNDS };
        
        foreach (string layerName in requiredLayers)
        {
            if (!PropertyExists(layersProp, 0, layersProp.arraySize, layerName))
            {
                SerializedProperty slot = GetEmptySlot(layersProp);
                if (slot != null)
                {
                    slot.stringValue = layerName;
                    slot.serializedObject.ApplyModifiedProperties();
                    layerAdded = true;
                    Debug.Log($"[NGO.Steam.Transporter] Layer eklendi: {layerName}");
                }
                else
                {
                    Debug.LogError($"[NGO.Steam.Transporter] Layer eklenemedi (Yer yok): {layerName}");
                }
            }
        }

        if (layerAdded)
        {
            tagManager.ApplyModifiedProperties();
            AssetDatabase.Refresh();
        }
    }
    
    private static void SetupPhysicsMatrix()
    {
        int boundsLayer = LayerMask.NameToLayer(LAYER_BOUNDS);
        
        if (boundsLayer == -1) return;
        
        for (int i = 0; i < 32; i++)
        {
            Physics.IgnoreLayerCollision(boundsLayer, i, true);
        }
    }

    private static bool PropertyExists(SerializedProperty property, int start, int end, string value)
    {
        for (int i = start; i < end; i++)
        {
            if (property.GetArrayElementAtIndex(i).stringValue.Equals(value)) return true;
        }
        return false;
    }

    private static SerializedProperty GetEmptySlot(SerializedProperty property)
    {
        for (int i = 6; i < property.arraySize; i++)
        {
            SerializedProperty t = property.GetArrayElementAtIndex(i);
            if (string.IsNullOrEmpty(t.stringValue))
            {
                return t;
            }
        }
        return null;
    }
}