using BattleTech;
using BattleTech.Designed;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace IRTweaks.Modules.UI
{
//    [HarmonyPatch(typeof(EncounterLayerData), "GetEncounterBoundaryTexture")]
//    static class MapBoundary
//    {
//		// TODO: Add real conditional gate here.
//		static bool Prepare() => true;

//        static void Prefix(EncounterLayerData __instance, 
//			ref Texture2D ___encounterBoundaryTexture, EncounterBoundaryChunkGameLogic ___encounterBoundaryChunk)
//        {
//			// Override the default texture composition if it's not already been created. The vanilla method will then return it.
//			if (___encounterBoundaryTexture == null)
//			{
//				int mapBoundaryWidth = SplatMapInfo.mapBoundaryWidth;
//				int num = 2048 - SplatMapInfo.mapBoundaryWidth;
//				Mod.Log.Info?.Write($"Generating boundary texture with mapBoundaryWidth:{mapBoundaryWidth}  num: {num}");

//				// Recalculate the rectHolders for the encounter boundaries
//				__instance.CalculateEncounterBoundary();

//				if (___encounterBoundaryChunk != null && ___encounterBoundaryChunk.encounterBoundaryRectList.Count > 0)
//				{
//					___encounterBoundaryTexture = new Texture2D(512, 512, TextureFormat.ARGB32, mipChain: false);
					
//					// Initialize the map to a flat black
//					Color[] pixels = ___encounterBoundaryTexture.GetPixels();
//					for (int i = 0; i < pixels.Length; i++)
//					{
//						pixels[i] = Color.black;
//					}
//					___encounterBoundaryTexture.SetPixels(pixels);

//					// Iterate the rectangle for each encounter boundary and paint it white, to allow
//					//   the texture to take effect
//					for (int rectIdx = 0; rectIdx < ___encounterBoundaryChunk.encounterBoundaryRectList.Count; rectIdx++)
//					{
//						Rect rect = ___encounterBoundaryChunk.encounterBoundaryRectList[rectIdx].rect;
//						int rectXOrigin = (int)rect.x;
//						int rectYOrigin = (int)rect.y;
//						Mod.Log.Info?.Write($"Texturing encounterBoundaryRect at idx: {rectIdx} with coordinates: {rectXOrigin}, {rectYOrigin} ");
//						Mod.Log.Info?.Write($"  -- painting pixels up to x=> {rectXOrigin + 1024 + rect.width}, y=> {rectYOrigin + 1024 + rect.height}");

//						for (int heightIdx = 0; (float)heightIdx < rect.height; heightIdx++)
//						{
//							for (int widthIdx = 0; (float)widthIdx < rect.width; widthIdx++)
//							{
//								int posX = widthIdx + 1024 + rectXOrigin;
//								int posY = heightIdx + 1024 + rectYOrigin;
//								if (mapBoundaryWidth < posX && posX < num && mapBoundaryWidth < posY && posY < num)
//								{
//									// Divide by 4 to get the corresponding position on the blit map. So the map will be 4 pixels wide when applied
//									//   a 2048 texture?
//									Mod.Log.Info?.Write($"Setting pixel at: {posX / 4}, {posY / 4} to white.");
//									//___encounterBoundaryTexture.SetPixel(posX / 4, posY / 4, Color.white);
//									___encounterBoundaryTexture.SetPixel(posX / 4, posY / 4, Color.red);

//								}
//							}
//						}
//					}
//					___encounterBoundaryTexture.Apply();

//					// Apply a shader (presumably that illuminates as you get closer?)
//					//Material mat = new Material(Shader.Find("Hidden/BT-ConvertToSDF"));
//					Material mat = new Material(Shader.Find("Standard"));

//                    // Blit the shader onto map and update mipmaps
//                    RenderTexture temporary = RenderTexture.GetTemporary(___encounterBoundaryTexture.width, ___encounterBoundaryTexture.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
//                    Graphics.Blit(___encounterBoundaryTexture, temporary, mat, 0);
//                    RenderTexture.active = temporary;
//                    ___encounterBoundaryTexture.ReadPixels(new Rect(0f, 0f, ___encounterBoundaryTexture.width, ___encounterBoundaryTexture.height), 0, 0);
//                    ___encounterBoundaryTexture.Apply(updateMipmaps: true, makeNoLongerReadable: false);

//                    RenderTexture.active = null;
//                }
//            }
//		}
//    }
}
