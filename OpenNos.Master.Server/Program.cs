﻿using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.ScsServices.Service;
using log4net;
using Microsoft.Owin.Hosting;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.DAL.EF.Helpers;
using OpenNos.Data;
using OpenNos.GameObject;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Interface;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Account = OpenNos.GameObject.Account;
using BoxInstance = OpenNos.GameObject.BoxInstance;
using Character = OpenNos.GameObject.Character;
using CharacterSkill = OpenNos.GameObject.CharacterSkill;
using Family = OpenNos.GameObject.Family;
using FamilyCharacter = OpenNos.GameObject.FamilyCharacter;
using ItemInstance = OpenNos.GameObject.ItemInstance;
using MapMonster = OpenNos.GameObject.MapMonster;
using MapNpc = OpenNos.GameObject.MapNpc;
using NpcMonster = OpenNos.GameObject.NpcMonster;
using NpcMonsterSkill = OpenNos.GameObject.NpcMonsterSkill;
using Portal = OpenNos.GameObject.Portal;
using Recipe = OpenNos.GameObject.Recipe;
using ScriptedInstance = OpenNos.GameObject.ScriptedInstance;
using Shop = OpenNos.GameObject.Shop;
using Skill = OpenNos.GameObject.Skill;
using SpecialistInstance = OpenNos.GameObject.SpecialistInstance;
using WearableInstance = OpenNos.GameObject.WearableInstance;

namespace OpenNos.Master.Server
{
    internal class Program
    {
        #region Members

        private static ManualResetEvent run = new ManualResetEvent(true);

        #endregion

        #region Methods

        private static void Main(string[] args)
        {
            try
            {
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

                // initialize Logger
                Logger.InitializeLogger(LogManager.GetLogger(typeof(Program)));

                var assembly = Assembly.GetExecutingAssembly();
                var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

                Console.Title = $"OpenNos Master Server";
                var ipAddress = ConfigurationManager.AppSettings["MasterIP"];
                var port = Convert.ToInt32(ConfigurationManager.AppSettings["MasterPort"]);
                var text = $"MASTER SERVER v{fileVersionInfo.ProductVersion}dev - PORT : {port} by SystemX64 Team";
                var offset = Console.WindowWidth / 2 + text.Length / 2;
                var separator = new string('=', Console.WindowWidth);
                Console.WriteLine(separator + string.Format("{0," + offset + "}\n", text) + separator);

                // initialize DB
                if (!DataAccessHelper.Initialize())
                {
                    Console.ReadLine();
                    return;
                }

                Logger.Log.Info(Language.Instance.GetMessageFromKey("CONFIG_LOADED"));

                try
                {
                    // register EF -> GO and GO -> EF mappings
                    RegisterMappings();

                    // configure Services and Service Host
                    var server = ScsServiceBuilder.CreateService(new ScsTcpEndPoint(ipAddress, port));
                    server.AddService<ICommunicationService, CommunicationService>(new CommunicationService());
                    server.ClientConnected += OnClientConnected;
                    server.ClientDisconnected += OnClientDisconnected;
                    WebApp.Start<Startup>(url: ConfigurationManager.AppSettings["WebAppURL"]);
                    server.Start();

                    // AUTO SESSION KICK
                    Observable.Interval(TimeSpan.FromMinutes(3)).Subscribe(x =>
                    {
                        Parallel.ForEach(MSManager.Instance.ConnectedAccounts.Replace(s => s.LastPulse.AddMinutes(3) <= DateTime.Now), connection =>
                        {
                            CommunicationServiceClient.Instance.KickSession(connection.AccountId, null);
                        });
                    });

                    CommunicationServiceClient.Instance.Authenticate(ConfigurationManager.AppSettings["MasterAuthKey"]);
                    Logger.Log.Info(Language.Instance.GetMessageFromKey("STARTED"));
                    Console.Title = $"OpenNos Master Server [ Channels: {MSManager.Instance.WorldServers.Count} - Players: {MSManager.Instance.ConnectedAccounts.Count} ]";
                }
                catch (Exception ex)
                {
                    Logger.Log.Error("General Error Server", ex);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("General Error", ex);
                Console.ReadKey();
            }
        }

        private static void OnClientConnected(object sender, ServiceClientEventArgs e)
        {
            Logger.Log.Info(Language.Instance.GetMessageFromKey("NEW_CONNECT") + e.Client.ClientId);
        }

        private static void OnClientDisconnected(object sender, ServiceClientEventArgs e)
        {
            Logger.Log.Info(Language.Instance.GetMessageFromKey("DISCONNECT") + e.Client.ClientId);
        }

        private static void RegisterMappings()
        {
            ////Prepare mappings for future use

            // register mappings for items
            DAOFactory.IteminstanceDAO.RegisterMapping(typeof(BoxInstance));
            DAOFactory.IteminstanceDAO.RegisterMapping(typeof(SpecialistInstance));
            DAOFactory.IteminstanceDAO.RegisterMapping(typeof(WearableInstance));
            DAOFactory.IteminstanceDAO.RegisterMapping(typeof(ItemInstance));
            DAOFactory.IteminstanceDAO.RegisterMapping(typeof(ItemInstanceDTO)).InitializeMapper();

            // entities
            DAOFactory.AccountDAO.RegisterMapping(typeof(Account)).InitializeMapper();
            DAOFactory.EquipmentOptionDAO.RegisterMapping(typeof(EquipmentOptionDTO)).InitializeMapper();
            DAOFactory.CharacterDAO.RegisterMapping(typeof(Character)).InitializeMapper();
            DAOFactory.CharacterRelationDAO.RegisterMapping(typeof(CharacterRelationDTO)).InitializeMapper();
            DAOFactory.CharacterSkillDAO.RegisterMapping(typeof(CharacterSkill)).InitializeMapper();
            DAOFactory.ComboDAO.RegisterMapping(typeof(ComboDTO)).InitializeMapper();
            DAOFactory.DropDAO.RegisterMapping(typeof(DropDTO)).InitializeMapper();
            DAOFactory.GeneralLogDAO.RegisterMapping(typeof(GeneralLogDTO)).InitializeMapper();
            DAOFactory.ItemDAO.RegisterMapping(typeof(ItemDTO)).InitializeMapper();
            DAOFactory.BazaarItemDAO.RegisterMapping(typeof(BazaarItemDTO)).InitializeMapper();
            DAOFactory.MailDAO.RegisterMapping(typeof(MailDTO)).InitializeMapper();
            DAOFactory.MapDAO.RegisterMapping(typeof(MapDTO)).InitializeMapper();
            DAOFactory.MapMonsterDAO.RegisterMapping(typeof(MapMonster)).InitializeMapper();
            DAOFactory.MapNpcDAO.RegisterMapping(typeof(MapNpc)).InitializeMapper();
            DAOFactory.FamilyDAO.RegisterMapping(typeof(FamilyDTO)).InitializeMapper();
            DAOFactory.FamilyCharacterDAO.RegisterMapping(typeof(FamilyCharacterDTO)).InitializeMapper();
            DAOFactory.FamilyLogDAO.RegisterMapping(typeof(FamilyLogDTO)).InitializeMapper();
            DAOFactory.MapTypeDAO.RegisterMapping(typeof(MapTypeDTO)).InitializeMapper();
            DAOFactory.MapTypeMapDAO.RegisterMapping(typeof(MapTypeMapDTO)).InitializeMapper();
            DAOFactory.NpcMonsterDAO.RegisterMapping(typeof(NpcMonster)).InitializeMapper();
            DAOFactory.NpcMonsterSkillDAO.RegisterMapping(typeof(NpcMonsterSkill)).InitializeMapper();
            DAOFactory.PenaltyLogDAO.RegisterMapping(typeof(PenaltyLogDTO)).InitializeMapper();
            DAOFactory.PortalDAO.RegisterMapping(typeof(PortalDTO)).InitializeMapper();
            DAOFactory.PortalDAO.RegisterMapping(typeof(Portal)).InitializeMapper();
            DAOFactory.QuicklistEntryDAO.RegisterMapping(typeof(QuicklistEntryDTO)).InitializeMapper();
            DAOFactory.RecipeDAO.RegisterMapping(typeof(Recipe)).InitializeMapper();
            DAOFactory.RecipeItemDAO.RegisterMapping(typeof(RecipeItemDTO)).InitializeMapper();
            DAOFactory.MinilandObjectDAO.RegisterMapping(typeof(MinilandObjectDTO)).InitializeMapper();
            DAOFactory.MinilandObjectDAO.RegisterMapping(typeof(MapDesignObject)).InitializeMapper();
            DAOFactory.RespawnDAO.RegisterMapping(typeof(RespawnDTO)).InitializeMapper();
            DAOFactory.RespawnMapTypeDAO.RegisterMapping(typeof(RespawnMapTypeDTO)).InitializeMapper();
            DAOFactory.ShopDAO.RegisterMapping(typeof(Shop)).InitializeMapper();
            DAOFactory.ShopItemDAO.RegisterMapping(typeof(ShopItemDTO)).InitializeMapper();
            DAOFactory.ShopSkillDAO.RegisterMapping(typeof(ShopSkillDTO)).InitializeMapper();
            DAOFactory.CardDAO.RegisterMapping(typeof(CardDTO)).InitializeMapper();
            DAOFactory.BCardDAO.RegisterMapping(typeof(BCardDTO)).InitializeMapper();
            DAOFactory.SkillDAO.RegisterMapping(typeof(Skill)).InitializeMapper();
            DAOFactory.MateDAO.RegisterMapping(typeof(MateDTO)).InitializeMapper();
            DAOFactory.MateDAO.RegisterMapping(typeof(Mate)).InitializeMapper();
            DAOFactory.TeleporterDAO.RegisterMapping(typeof(TeleporterDTO)).InitializeMapper();
            DAOFactory.StaticBonusDAO.RegisterMapping(typeof(StaticBonusDTO)).InitializeMapper();
            DAOFactory.FamilyDAO.RegisterMapping(typeof(Family)).InitializeMapper();
            DAOFactory.FamilyCharacterDAO.RegisterMapping(typeof(FamilyCharacter)).InitializeMapper();
            DAOFactory.ScriptedInstanceDAO.RegisterMapping(typeof(ScriptedInstanceDTO)).InitializeMapper();
            DAOFactory.ScriptedInstanceDAO.RegisterMapping(typeof(ScriptedInstance)).InitializeMapper();
        }

        #endregion
    }
}