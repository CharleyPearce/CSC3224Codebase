
using System;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Direction
{
    up, down, left, right
}

public class Doorway
{
    public int width;
    public Direction direction;
    public Doorway(int width, Direction direction)
    {
        this.width = width;
        this.direction = direction;
    }
}

public class Room
{
    // hold references to all the neighbours of this room
    public Room[] neighbours = new Room[4];
    
    public enum Type
    {
        start, fight, item, end, boss
    }

    public enum Direction
    {
        up, right, down, left
    }

    public Room getNeigbor(Direction direction)
    {
        return neighbours[(int)direction];
    }

    public void setNeigbor(Room room, Direction direction)
    {

    }

    // points to which room this room was generated from
    public int parentRoom;

    Type type;


    public Vector3Int enterDoor;
    public Vector3Int exitDoor;

    public Vector3Int centre;
    public int width;
    public int height;

    public int x_low = 0;
    public int x_high = 0;
    public int y_low = 0;
    public int y_high = 0;

    public Room(int x_coord, int y_coord, int width, int height)
    {
        centre = new Vector3Int(x_coord, y_coord, 0);
        this.width = width;
        this.height = height;

        x_low = x_coord - width / 2;
        y_low = y_coord - height / 2;
        x_high = x_coord + width / 2;
        y_high = y_coord + height / 2;

        enterDoor = new Vector3Int(centre.x - (width / 2), centre.y - (height / 2) + 1, 0);
        exitDoor = new Vector3Int(centre.x + (width / 2), centre.y - (height / 2) + 1, 0);
    }

    public void setType(Type type)
    {
        this.type = type;
    }

    public Type getType()
    {
        return type;
    }
}

public class TileGenerator : MonoBehaviour
{

    public GameObject[] Enemies;
    public GameObject[] Backgrounds;
    public GameObject[] Items;
    GameObject cam;
    bool bossRoom;
    public GameObject entitiesParent;
    public GameObject platform;
    public Tilemap walls;
    public Tilemap background;
    public Tilemap entities;

    // public Tile ruleTile;
    public Tile bgtile;

    public RuleTile ruleTile;

    public GameObject goblet;

    Vector3Int worldCentre;

    long loadTime = 0;

    int xlowest;
    int xhighest;
    int ylowest;
    int yhighest;

    System.Random rng;

    List<Room> rooms = new List<Room>();
    public int noRooms;

    void Start()
    {
        cam = GameObject.Find("Main Camera");
        // add a background according to the floor
        if (PlayerPrefs.HasKey("floorNo") && PlayerPrefs.GetInt("floorNo") < Backgrounds.Length)
        {
            Instantiate(Backgrounds[PlayerPrefs.GetInt("floorNo")], new Vector3(0, 0, 20), Quaternion.identity, cam.transform);
        }
        else
        {
            PlayerPrefs.SetInt("floorNo", 0);
            Instantiate(Backgrounds[0], new Vector3(0, 0, 20), Quaternion.identity, cam.transform);
        }
        if (PlayerPrefs.GetInt("floorNo") == 2)
        {
            bossRoom = true;
        } else
        {
            bossRoom = false;
        }

        if (PlayerPrefs.HasKey("noRooms"))
        {
            noRooms = PlayerPrefs.GetInt("noRooms");
        }
        else
        {
            noRooms = 5;
        }

        loadTime = Generate();
    }

    public float getLoadTime()
    {
        return loadTime;
    }

    public void clearMap()
    {
        // clear entities
        foreach (Transform child in entitiesParent.transform)
        {
            Destroy(child.gameObject);
        }

        background.ClearAllTiles();
        walls.ClearAllTiles();
        // entities.ClearAllTiles();

        xlowest = 0;
        xhighest = 0;
        ylowest = 0;
        yhighest = 0;
        rooms.Clear();
    }

    public long Generate()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        float startTime = Time.time;
        rng = new System.Random();

        worldCentre = walls.WorldToCell(transform.position);

        // Room startroom = new Room(0, 0, 11, 7);
        rooms.Add(new Room(0, 0, 9, 7));
        rooms[0].parentRoom = -1;
        updateLimits(rooms[0]);
        rooms[0].setType(Room.Type.start);

        for (int i = 1; i < noRooms - 1; i++)
        {
            rooms.Add(generateNeighbor(rooms[i-1]));
            rooms[i].setType(Room.Type.fight);
        }
        
        // set the room half way through the dungeon as an item room
        rooms[rooms.Count/2].setType(Room.Type.item);

        // add the boss on the third floor
        if (bossRoom)
        {
            rooms.Add(generateNeighbor(rooms[noRooms - 2], 15, 10, true));
            rooms[noRooms - 1].setType(Room.Type.boss);
        }
        else
        {
            rooms.Add(generateNeighbor(rooms[noRooms - 2]));
            rooms[noRooms - 1].setType(Room.Type.end);
        }



        drawBackground();

        for (int i = 0; i < rooms.Count; i++)
        {
            drawroom(rooms[i]);
        }

        for (int i = 0; i < rooms.Count; i++)
        {
            fillRoom(rooms[i]);
        }

        stopwatch.Stop();

        return stopwatch.ElapsedMilliseconds;
    }

    Room generateNeighbor(Room room, int width=0, int height=0, bool bossRoom = false)
    {
        if (width == 0)
        {
            width = rng.Next(1, 6) + rng.Next(1, 6) + rng.Next(0, 6);
        }
        if (height == 0)
        {
            height = rng.Next(2, 6) + rng.Next(2, 6);
        }

        int x;
        int y;

        int direction = rng.Next(0, 4);
        int directionModifier = 0;
        int newDirection = direction;
        bool foundSpace = false;

        x = 0;
        y = 0;

        while (!foundSpace)
        {
            if (!bossRoom)
            {
                newDirection = (direction + directionModifier) % 4;
            } else
            {
                // ensures that the player has to descend into the bossroom
                directionModifier = 4;
                newDirection = 2;
            }

            if (newDirection == 0 && room.neighbours[newDirection] == null)
            {
                // room above
                y = room.centre.y + room.height / 2 + height / 2;
                x = room.centre.x;
                foundSpace = checkArea(x, y, width, height);
            }
            else if (newDirection == 1 && room.neighbours[newDirection] == null)
            {
                // room right
                x = room.centre.x + room.width / 2 + width / 2;
                y = room.centre.y;
                foundSpace = checkArea(x, y, width, height);
            }
            else if (newDirection == 2 && room.neighbours[newDirection] == null)
            {
                // room below
                y = room.centre.y - room.height / 2 - height / 2;
                x = room.centre.x;
                foundSpace = checkArea(x, y, width, height);
            }
            else if (newDirection == 3 && room.neighbours[newDirection] == null)
            {
                // room left
                x = room.centre.x - room.width / 2 - width / 2;
                y = room.centre.y;
                foundSpace = checkArea(x, y, width, height);
            }

            if (!foundSpace)
            {
                if (directionModifier == 4)
                {
                    if (room.parentRoom == -1)
                    {
                        // if a room can't be generated at the origin room, generate a new neigbor at a random room 
                        return generateNeighbor(rooms[rng.Next(0, rooms.Count)]);
                    }
                    else
                    {
                        // generate a neigbour based on the previous room if this room can have any
                        return generateNeighbor(room.neighbours[room.parentRoom], width, height, bossRoom);
                    }
                }
                else
                {
                    directionModifier++;
                }
            }

        }

        Room newRoom = new Room(x, y, width, height);

        updateLimits(newRoom);

        room.neighbours[newDirection] = newRoom;
        newRoom.neighbours[(newDirection + 2) % 4] = room;
        newRoom.parentRoom = (newDirection + 2) % 4;
        return newRoom;
    }

    // check if there is enough space to add a room TODO
    bool checkArea(int x, int y, int width, int height)
    {
        foreach (var room in rooms)
        {
            if (Math.Abs(x - room.centre.x) < (width + room.width) / 2)
            {
                if (Math.Abs(y - room.centre.y) < (height + room.height) / 2)
                {
                    return false;
                }
            }
        }

        return true;
    }

    void updateLimits(Room room)
    {
        if (room.x_high > xhighest)
            xhighest = room.x_high;
        if (room.x_low < xlowest)
            xlowest = room.x_low;
        if (room.y_high > yhighest)
            yhighest = room.y_high;
        if (room.y_low < ylowest)
            ylowest = room.y_low;
    }

    void drawBackground()
    {
        Vector3Int newPos;
        for (int i = xlowest - 10; i < xhighest + 10; i++)
        {
            for (int j = ylowest - 10; j < yhighest + 10; j++)
            {
                newPos = new Vector3Int(worldCentre.x + i, worldCentre.y + j, worldCentre.z);
                walls.SetTile(newPos, ruleTile);
            }

        }
    }

    void drawroom(Room room)
    {
        // Vector3Int roomCentre = new Vector3Int(worldCentre.x + room.centre.x, worldCentre.y + room.centre.y, worldCentre.z);
        Vector3Int roomCentre = room.centre + worldCentre;

        Vector3Int currentpos;

        for (int x = -room.width / 2; x <= room.width / 2; x++)
        {
            for (int y = -room.height / 2; y <= room.height / 2; y++)
            {
                currentpos = new Vector3Int(roomCentre.x + x, roomCentre.y + y, roomCentre.z);

                if (Math.Abs(x) == room.width / 2 || Math.Abs(y) == room.height / 2)
                {
                    // walls.SetTile(currentpos, ruleTile);
                }
                else
                {

                    walls.SetTile(currentpos, null);
                }
            }
        }

        if (room.neighbours[0] != null)
        {
            currentpos = new Vector3Int(roomCentre.x, room.y_high + worldCentre.y, roomCentre.z);
            walls.SetTile(currentpos, null);
            background.SetTile(currentpos, null);
        }
        if (room.neighbours[1] != null)
        {
            currentpos = new Vector3Int(room.x_high + worldCentre.x, roomCentre.y, roomCentre.z);
            walls.SetTile(currentpos, null);
            background.SetTile(currentpos, null);
            currentpos -= new Vector3Int(0, 1, 0);
            walls.SetTile(currentpos, null);
        }
        if (room.neighbours[2] != null)
        {
            currentpos = new Vector3Int(roomCentre.x, room.y_low + worldCentre.y, roomCentre.z);
            walls.SetTile(currentpos, null);
            // background.SetTile(currentpos, null);
        }
        if (room.neighbours[3] != null)
        {
            currentpos = new Vector3Int(room.x_low + worldCentre.x, roomCentre.y, roomCentre.z);
            walls.SetTile(currentpos, null);
            currentpos -= new Vector3Int(0, 1, 0);
            walls.SetTile(currentpos, null);
            // background.SetTile(currentpos, null);
        }
    }

    void fillRoom(Room room)
    {
        if (room.neighbours[0] != null)
        {
            if (room.height < 8)
            {
                Instantiate(platform, new Vector3(0.5f, 0.5f, 0) + room.centre + worldCentre, Quaternion.identity, entitiesParent.transform);
            }
            else
            {
                for (int i = -room.height / 2; i < room.height / 2 - 1; i += 3)
                {
                    Instantiate(platform, new Vector3(0.5f, 0.5f, 0) + room.centre + worldCentre + new Vector3Int(0, i, 0), Quaternion.identity, entitiesParent.transform);
                }
            }

        }

        if (room.neighbours[2] != null)
        {
            Instantiate(platform, new Vector3(0.5f, 0.5f, 0) + room.centre + worldCentre + new Vector3Int(0, -room.height / 2, 0), Quaternion.identity, entitiesParent.transform);
        }

        if (room.neighbours[1] != null)
        {
            if (room.height / 2 > 6)
            {
                for (int i = -room.height / 2; i < -1; i += 3)
                {
                    Instantiate(platform, new Vector3(0.5f, 0.5f, 0) + room.centre + worldCentre + new Vector3Int(room.width / 2 - 1, i, 0), Quaternion.identity, entitiesParent.transform);
                }
            }
        }

        if (room.neighbours[3] != null)
        {
            if (room.height / 2 > 6)
            {
                for (int i = -room.height / 2; i < -1; i += 3)
                {
                    Instantiate(platform, new Vector3(0.5f, 0.5f, 0) + room.centre + worldCentre + new Vector3Int(-room.width / 2 + 1, i, 0), Quaternion.identity, entitiesParent.transform);
                }
            }
        }
        if (room.getType() == Room.Type.end)
        {
            Instantiate(goblet, new Vector3(0.5f, 0.5f, 0) + room.centre + worldCentre, Quaternion.identity, entitiesParent.transform);
        }

        if (room.getType() == Room.Type.item)
        {
            Instantiate(Items[0], new Vector3(0.5f, 0.5f, -5) + room.centre + worldCentre, Quaternion.identity, entitiesParent.transform);
        }

        if (room.getType() == Room.Type.fight)
        {
            GenerateEnemies(room);
        }

        if (room.getType() == Room.Type.boss)
        {
            BossRoom(room);
        }
    }

    public void BossRoom(Room room)
    {
        
        Vector3 pos = room.centre;
        Instantiate(Enemies[3], pos + worldCentre, Quaternion.identity, entitiesParent.transform);


    }

    public void GenerateEnemies(Room room)
    {
        Vector3 pos;
        int x = room.centre.x;
        int y = room.y_low+1;
        // add an enemy for each 4 blocks of width
        for (int i = 1; i <= room.width/4; i++)
        {
            // generate a new x value so that the enemy doesn't spawn in the centre 
            x = rng.Next(room.x_low + 1, room.x_high -1);
            while (x == room.centre.x)
            {
                x = rng.Next(room.x_low + 1, room.x_high - 1);
            }
            pos = new Vector3(x + 0.5f, y + 2, 0);

            // generate a random number to define the type of enemy, skewed so that Enemies[0] is the most common
            int rand = UnityEngine.Random.Range(0, 15);
            if (rand < 2)
            {
                Instantiate(Enemies[2], pos + worldCentre, Quaternion.identity, entitiesParent.transform);
            } else if (rand < 4)
            {
                Instantiate(Enemies[1], pos + worldCentre, Quaternion.identity, entitiesParent.transform);
            } else
            {
                Instantiate(Enemies[0], pos + worldCentre, Quaternion.identity, entitiesParent.transform);
            }
        }
    }
}
