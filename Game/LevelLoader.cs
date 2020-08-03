using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;

namespace RewindGame.Game
{
    public enum TileType
    {
        intangible,
        solid,
        death,
        platform,
        freezetime
    }

    public enum EntityType
    {
        Spawnpoint
    }
    class RawLevel
    {
        public string ogmoVersion;
        public int width;
        public int height;
        public int offsetX;
        public int offsetY;
        public List<RawLayers> layers;
    }
    class RawLayers
    {
        public string name;
        public string _eid;
        public int offsetX;
        public int offsetY;
        public int gridCellWidth;
        public int gridCellHeight;
        public int gridCellsX;
        public int gridCellsY;
        public string tileset;
        public List<int> data;
        public int exportMode;
        public int arrayMode;
        public List<RawEntities> entities;

    }
    class RawEntities
    {
        public string name;
        public int id;
        public string _eid;
        public int x;
        public int y;
        public int originX;
        public int originY;
        public bool flippedX;
    }
    class LevelLoader
    {
        // we pass in a level here to populate it with everything
        public static void RenderTile()
        {
            //Maybe use later?
        }
        public static void LoadLevel(String fileName, Level unloadedLevel)
        {
            string levelStuff;
            using (StreamReader sr = new StreamReader("Content/levels/"+fileName))
            {
                 levelStuff = sr.ReadToEnd();
            }
            var newLevel = JsonConvert.DeserializeObject<RawLevel>(levelStuff);
            foreach(RawLayers layer in newLevel.layers)
            {
                if(layer.entities!=null)
                {
                    foreach (RawEntities entity in layer.entities)
                    {
                        switch (entity.name)
                        {
                            case "Spawnpoint":
                                unloadedLevel.PlaceEntity(EntityType.Spawnpoint, new Vector2(entity.x, entity.y));
                                break;
                        }
                    }
                }
            }
            int dataCounter = 0;
            int dataXCounter = 0;
            int dataYCounter = 0;
            while (dataCounter < newLevel.layers[2].data.Count)
            {
                if(newLevel.layers[2].data[dataCounter] > -1)
                {
                    switch (newLevel.layers[2].data[dataCounter]) 
                    {
                        default:
                            unloadedLevel.PlaceTile(TileType.solid, dataXCounter, dataYCounter, new TileSprite());
                            break;
                    }
                    dataCounter++;
                    dataXCounter++;
                    if(dataXCounter > newLevel.layers[2].gridCellsX)
                    {
                        dataXCounter = 0;
                        dataYCounter++;
                    }
                }
            }
            dataCounter = 0;
            dataXCounter = 0;
            dataYCounter = 0;
            while (dataCounter < newLevel.layers[0].data.Count)
            {
                if (newLevel.layers[0].data[dataCounter] > -1)
                {
                    switch (newLevel.layers[0].data[dataCounter])
                    {
                        default:
                            unloadedLevel.PlaceDecorative(dataXCounter, dataYCounter, new TileSprite());
                            break;
                    }
                    dataCounter++;
                    dataXCounter++;
                    if (dataXCounter > newLevel.layers[0].gridCellsX)
                    {
                        dataXCounter = 0;
                        dataYCounter++;
                    }
                }
            }
            dataCounter = 0;
            dataXCounter = 0;
            dataYCounter = 0;
            while (dataCounter < newLevel.layers[3].data.Count)
            {
                if (newLevel.layers[3].data[dataCounter] > -1)
                {
                    switch (newLevel.layers[3].data[dataCounter])
                    {
                        default:
                            unloadedLevel.PlaceDecorative(dataXCounter, dataYCounter, new TileSprite());
                            break;
                    }
                    dataCounter++;
                    dataXCounter++;
                    if (dataXCounter > newLevel.layers[3].gridCellsX)
                    {
                        dataXCounter = 0;
                        dataYCounter++;
                    }
                }
            }
            // call level.placeTile, level.placeEntity, level.placeDecorative
            //todo
        }

    }
}
