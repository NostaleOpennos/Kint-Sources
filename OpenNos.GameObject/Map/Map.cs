﻿using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.PathFinder;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OpenNos.GameObject
{
    public class Map : IMapDTO
    {
        #region Members

        private readonly Random random;

        #endregion

        #region Instantiation

        public Map(short mapId, byte[] data)
        {
            random = new Random();
            MapId = mapId;
            Data = data;
            LoadZone();
            MapTypes = new List<MapTypeDTO>();
            foreach (MapTypeMapDTO maptypemap in DAOFactory.MapTypeMapDAO.Where(s => s.MapId == mapId).ToList())
            {
                var maptype = DAOFactory.MapTypeDAO.FirstOrDefault(s => s.MapTypeId == maptypemap.MapTypeId);
                MapTypes.Add(maptype);
            }

            if (MapTypes.Any())
            {
                if (MapTypes.ElementAt(0).RespawnMapTypeId != null)
                {
                    long? respawnMapTypeId = MapTypes.ElementAt(0).RespawnMapTypeId;
                    long? returnMapTypeId = MapTypes.ElementAt(0).ReturnMapTypeId;
                    if (respawnMapTypeId != null)
                    {
                        DefaultRespawn = DAOFactory.RespawnMapTypeDAO.FirstOrDefault(s => s.RespawnMapTypeId == respawnMapTypeId);
                    }

                    if (returnMapTypeId != null)
                    {
                        DefaultReturn = DAOFactory.RespawnMapTypeDAO.FirstOrDefault(s => s.RespawnMapTypeId == returnMapTypeId);
                    }
                }
            }
        }

        #endregion

        #region Properties

        public byte[] Data { get; set; }

        public RespawnMapTypeDTO DefaultRespawn { get; }

        public RespawnMapTypeDTO DefaultReturn { get; }

        public GridPos[,] Grid { get; private set; }

        public short MapId { get; set; }

        public List<MapTypeDTO> MapTypes { get; }

        public int Music { get; set; }

        /// <summary>
        /// This list ONLY for READ access to MapMonster, you CANNOT MODIFY them here. Use
        /// Add/RemoveMonster instead.
        /// </summary>
        public string Name { get; set; }

        public bool ShopAllowed { get; set; }

        private ConcurrentBag<MapCell> Cells { get; set; }

        private int XLength { get; set; }

        private int YLength { get; set; }

        #endregion

        #region Methods

        public static int GetDistance(Character character1, Character character2)
        {
            return GetDistance(new MapCell { X = character1.PositionX, Y = character1.PositionY }, new MapCell { X = character2.PositionX, Y = character2.PositionY });
        }

        public static int GetDistance(MapCell p, MapCell q)
        {
            return (int)Heuristic.Octile(Math.Abs(p.X - q.X), Math.Abs(p.Y - q.Y));
        }

        public ConcurrentBag<MonsterToSummon> GenerateMonsters(short vnum, short amount, bool move, List<EventContainer> deathEvents, bool isBonus = false, bool isHostile = true, bool isBoss = false)
        {
            ConcurrentBag<MonsterToSummon> summonParameters = new ConcurrentBag<MonsterToSummon>();
            for (int i = 0; i < amount; i++)
            {
                var cell = GetRandomPosition();
                summonParameters.Add(new MonsterToSummon(vnum, cell, -1, move, isBonus: isBonus, isHostile: isHostile, isBoss: isBoss) { DeathEvents = deathEvents });
            }

            return summonParameters;
        }

        public List<NpcToSummon> GenerateNpcs(short vnum, short amount, ConcurrentBag<EventContainer> deathEvents, bool isMate, bool isProtected)
        {
            List<NpcToSummon> summonParameters = new List<NpcToSummon>();
            for (int i = 0; i < amount; i++)
            {
                var cell = GetRandomPosition();
                summonParameters.Add(new NpcToSummon(vnum, cell, -1, deathEvents, isMate: isMate, isProtected: isProtected));
            }

            return summonParameters;
        }

        public MapCell GetRandomPosition()
        {
            if (Cells != null)
            {
                return Cells.OrderBy(s => random.Next(int.MaxValue)).FirstOrDefault();
            }

            Cells = new ConcurrentBag<MapCell>();
            Parallel.For(0, YLength, y => Parallel.For(0, XLength, x =>
            {
                if (!IsBlockedZone(x, y))
                {
                    Cells.Add(new MapCell { X = (short)x, Y = (short)y });
                }
            }));
            return Cells.OrderBy(s => random.Next(int.MaxValue)).FirstOrDefault();
        }

        public bool IsArenaPVPable(int x, int y)
        {
            try
            {
                if (Grid == null)
                {
                    return false;
                }

                return !Grid[x, y].IsArenaStairs();
            }
            catch
            {
                return false;
            }
        }

        public bool IsBlockedZone(int x, int y)
        {
            try
            {
                if (Grid == null)
                {
                    return false;
                }

                return !Grid[x, y].IsWalkable();
            }
            catch
            {
                return true;
            }
        }

        internal bool GetFreePosition(ref short firstX, ref short firstY, byte xpoint, byte ypoint)
        {
            var MinX = (short)(-xpoint + firstX);
            var MaxX = (short)(xpoint + firstX);

            var MinY = (short)(-ypoint + firstY);
            var MaxY = (short)(ypoint + firstY);

            List<MapCell> cells = new List<MapCell>();
            for (short y = MinY; y <= MaxY; y++)
            {
                for (short x = MinX; x <= MaxX; x++)
                {
                    if (x != firstX || y != firstY)
                    {
                        cells.Add(new MapCell { X = x, Y = y });
                    }
                }
            }

            foreach (MapCell cell in cells.OrderBy(s => random.Next(int.MaxValue)))
            {
                if (IsBlockedZone(firstX, firstY, cell.X, cell.Y))
                {
                    continue;
                }

                firstX = cell.X;
                firstY = cell.Y;
                return true;
            }

            return false;
        }

        private bool IsBlockedZone(int firstX, int firstY, int mapX, int mapY)
        {
            for (int i = 1; i <= Math.Abs(mapX - firstX); i++)
            {
                if (IsBlockedZone(firstX + Math.Sign(mapX - firstX) * i, firstY))
                {
                    return true;
                }
            }

            for (int i = 1; i <= Math.Abs(mapY - firstY); i++)
            {
                if (IsBlockedZone(firstX, firstY + Math.Sign(mapY - firstY) * i))
                {
                    return true;
                }
            }

            return false;
        }

        private void LoadZone()
        {
            // TODO: Optimize
            using (Stream stream = new MemoryStream(Data))
            {
                const int NUM_BYTES_TO_READ = 1;
                const int NUM_BYTES_READ = 0;
                byte[] bytes = new byte[NUM_BYTES_TO_READ];

                byte[] xlength = new byte[2];
                byte[] ylength = new byte[2];
                stream.Read(bytes, NUM_BYTES_READ, NUM_BYTES_TO_READ);
                xlength[0] = bytes[0];
                stream.Read(bytes, NUM_BYTES_READ, NUM_BYTES_TO_READ);
                xlength[1] = bytes[0];
                stream.Read(bytes, NUM_BYTES_READ, NUM_BYTES_TO_READ);
                ylength[0] = bytes[0];
                stream.Read(bytes, NUM_BYTES_READ, NUM_BYTES_TO_READ);
                ylength[1] = bytes[0];
                YLength = BitConverter.ToInt16(ylength, 0);
                XLength = BitConverter.ToInt16(xlength, 0);

                Grid = new GridPos[XLength, YLength];
                for (short i = 0; i < YLength; ++i)
                {
                    for (short t = 0; t < XLength; ++t)
                    {
                        stream.Read(bytes, NUM_BYTES_READ, NUM_BYTES_TO_READ);
                        Grid[t, i] = new GridPos()
                        {
                            Value = bytes[0],
                            X = t,
                            Y = i,
                        };
                    }
                }
            }
        }

        #endregion
    }
}