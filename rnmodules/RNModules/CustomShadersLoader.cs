using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace RNModules
{
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class CustomShadersLoader : MonoBehaviour, IDisposable
    {
        private static bool _completed;

        private static void Log(string msg, params object[] args)
        {
            Debug.Log(string.Format(msg, args));
        }

        public void Start()
        {
            if (!CustomShadersLoader._completed)
            {
                CustomShadersLoader.Log("CustomShadersLoader::Start", new object[0]);
                Dictionary<string, Material> materials = new Dictionary<string, Material>();
                UrlDir.UrlConfig[] configs = GameDatabase.Instance.GetConfigs("CustomPartShader");
                UrlDir.UrlConfig[] array = configs;
                for (int i = 0; i < array.Length; i++)
                {
                    UrlDir.UrlConfig config = array[i];
                    string partName = config.config.GetValue("partName");
                    bool applyShaderToAllMeshes = false;
                    if (config.config.HasValue("applyShaderToAllMeshes"))
                    {
                        applyShaderToAllMeshes = bool.Parse(config.config.GetValue("applyShaderToAllMeshes"));
                    }
                    CustomShadersLoader.Log("Part {0}, AllMeshes {1}", new object[]
                    {
                        partName,
                        applyShaderToAllMeshes
                    });
                    AvailablePart model = PartLoader.Instance.parts.FirstOrDefault((AvailablePart x) => x.name == partName.Replace("_", "."));
                    if (model == null)
                    {
                        CustomShadersLoader.Log("Model '{0}' is not found", new object[]
                        {
                            partName
                        });
                    }
                    else
                    {
                        MeshRenderer[] renderers = model.partPrefab.FindModelComponents<MeshRenderer>().ToArray();
                        string arg_171_0 = "Meshes: {0}";
                        object[] array2 = new object[1];
                        array2[0] = string.Join(", ", (from x in renderers
                                                       select string.Format("'{0}'", x.name)).ToArray<string>());
                        CustomShadersLoader.Log(arg_171_0, array2);
                        if (applyShaderToAllMeshes)
                        {
                            string shaderFileName = config.config.GetValue("shaderFileName");
                            Material material = CustomShadersLoader.LoadShader(materials, shaderFileName);
                            MeshRenderer[] array3 = renderers;
                            for (int j = 0; j < array3.Length; j++)
                            {
                                MeshRenderer meshRenderer = array3[j];
                                this.AssignMaterialToMesh(meshRenderer, config.config.GetNode("Textures"), material);
                            }
                        }
                        else
                        {
                            ConfigNode[] meshNodes = config.config.GetNodes("Mesh");
                            ConfigNode[] array4 = meshNodes;
                            for (int j = 0; j < array4.Length; j++)
                            {
                                ConfigNode meshNode = array4[j];
                                string meshName = meshNode.GetValue("meshName");
                                string shaderFileName = meshNode.GetValue("shaderFileName");
                                MeshRenderer meshRenderer = renderers.FirstOrDefault((MeshRenderer x) => x.name == meshName);
                                if (meshRenderer == null)
                                {
                                    CustomShadersLoader.Log("Unable to find mesh '{0}'", new object[]
                                    {
                                        meshName
                                    });
                                }
                                else
                                {
                                    Material material = CustomShadersLoader.LoadShader(materials, shaderFileName);
                                    this.AssignMaterialToMesh(meshRenderer, meshNode.GetNode("Textures"), material);
                                }
                            }
                        }
                    }
                }
                CustomShadersLoader._completed = true;
            }
        }

        private static Material LoadShader(IDictionary<string, Material> materials, string shaderFileName)
        {
            if (!materials.ContainsKey(shaderFileName))
            {
                string shaderText = File.ReadAllText(shaderFileName);
                materials.Add(shaderFileName, new Material(Shader.Find("shaderText")));
            }
            return materials[shaderFileName];
        }

        private void AssignMaterialToMesh(MeshRenderer mesh, ConfigNode textures, Material material)
        {
            mesh.material = (Material)UnityEngine.Object.Instantiate(material);
            IEnumerator enumerator = null;
            try
            {
                enumerator = textures.values.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ConfigNode.Value value = (ConfigNode.Value)enumerator.Current;
                    GameDatabase.TextureInfo texture = GameDatabase.Instance.databaseTexture.FirstOrDefault((GameDatabase.TextureInfo x) => x.name == value.value);
                    if (texture == null)
                    {
                        CustomShadersLoader.Log("Unable to find texture '{0}'", new object[]
                        {
                            value.value
                        });
                    }
                    else
                    {
                        mesh.material.SetTexture(value.name, texture.isNormalMap ? texture.normalMap : texture.texture);
                    }
                }
            }
            finally
            {
                if (enumerator != null)
                {
                    Dispose();
                }
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
