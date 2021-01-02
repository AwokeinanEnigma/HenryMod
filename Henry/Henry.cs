using BepInEx;
using EntityStates;
using Henry.HenryStates;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using System;
using System.Reflection;
using UnityEngine;

namespace Henry
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.TeamCloudburst.Cloudburst", BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(guid, modName, version)]
    [R2APISubmoduleDependency(new string[]
    {
        //Needed to register the character
        "SurvivorAPI",
        //Needed to load assets
        "ResourcesAPI",
        //Needed for prefab builder
        "PrefabAPI",
        //Needed for language tokens
        "LanguageAPI",
        //NEeded for buffs
        "BuffAPI",
        //Needed for registering skills
        "LoadoutAPI",
    })]

    public class Plugin : BaseUnityPlugin
    {
        public const string guid = "com.IGiveEnigmaAllRightsToMyMod.Henry";
        public const string modName = "Henry";
        public const string version = "1.0.0";

        public static Plugin instance;

        /// <summary>
        /// Called BEFORE the first frame of the game.
        /// </summary>
        public static event Action awake;
        /// <summary>
        /// Called on the first frame of the game.
        /// </summary>
        public static event Action start;
        /// <summary>
        /// Called when the mod is enabled
        /// </summary>
        public static event Action onEnable;
        /// <summary>
        /// Called when the mod is disabled
        /// </summary>
        public static event Action onDisable;
        /// <summary>
        /// Called on the mod's FixedUpdate
        /// </summary>
        public static event Action onFixedUpdate;

        public GameObject HenryBody;

        public CharacterBody characterBody;
        public SkillLocator skillLocator;

        public Plugin()
        {
            Log.logger = Logger;



            //:)
            Assembly[] assembly = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assembly.Length; i++) {
                var currentASM = assembly[i];

                var builder = currentASM.GetType("PrefabBuilder");
                if (builder != null)
                {
                    //BepInEx.Bootstrap.Chainloader.ManagerObject.AddComponent<Utils.Observer>();
                    //var a = currentASM.GetType(typeof(BaseUnityPlugin)).GetType;
                    Log.LogI(currentASM.FullName + " is using " + builder);
                }
            }


            new Assets();

            CreateHenry();


            Log.LogM("Henry loaded!");
        }


        private void CreateHenry()
        {
            //On.RoR2.Networking.GameNetworkManager.OnClientConnect += (self, user, t) => { };

            CreateHenryPrefab();
            SetComponents();
            SetSkills();
            CreateSurvivorDef();
        }

        private void CreateHenryPrefab()
        {
            if (!HenryBody)
            {
                PrefabBuilder builder = new PrefabBuilder();
                builder.prefabName = "HenryBody";
                builder.masteryAchievementUnlockable = "";
                builder.model = Assets.mainAssetBundle.LoadAsset<GameObject>("mdlHenry");
                builder.defaultSkinIcon = LoadoutAPI.CreateSkinIcon(API.HexToColor("00A86B"), API.HexToColor("E56717"), API.HexToColor("D9DDDC"), API.HexToColor("43464B"));
                builder.masterySkinIcon = LoadoutAPI.CreateSkinIcon(API.HexToColor("00A86B"), API.HexToColor("E56717"), API.HexToColor("D9DDDC"), API.HexToColor("43464B"));
                builder.masterySkinDelegate = material;

                HenryBody = builder.CreatePrefab();
                Material material()
                {
                    return Assets.mainAssetBundle.LoadAsset<Material>("matHenryAlt");

                }
            }
        }

        #region Components
        private void SetComponents()
        {
            skillLocator = HenryBody.GetComponent<SkillLocator>();
            characterBody = HenryBody.GetComponent<CharacterBody>();


            SetCharacterBody();
        }

        private void SetCharacterBody()
        {
            characterBody.baseAcceleration = 70f;
            characterBody.baseArmor = 20; //Base armor this character has, set to 20 if this character is melee 
            characterBody.baseAttackSpeed = 1; //Base attack speed, usually 1
            characterBody.baseCrit = 1;  //Base crit, usually one
            characterBody.baseDamage = 12; //Base damage
            characterBody.baseJumpCount = 1; //Base jump amount, set to 2 for a double jump. 
            characterBody.baseJumpPower = 16; //Base jump power
            characterBody.baseMaxHealth = 150; //Base health, basically the health you have when you start a new run
            characterBody.baseMaxShield = 0; //Base shield, basically the same as baseMaxHealth but with shields
            characterBody.baseMoveSpeed = 7; //Base move speed, this is usual 7
            characterBody.baseNameToken = "HENRY_BODY_NAME"; //The base name token. 
            characterBody.subtitleNameToken = "HENRY_BODY_SUBTITLE"; //Set this if its a boss
            characterBody.baseRegen = 1.5f; //Base health regen.
            characterBody.bodyFlags = (CharacterBody.BodyFlags.ImmuneToExecutes | CharacterBody.BodyFlags.Mechanical); ///Base body flags, should be self explanatory 
            characterBody.crosshairPrefab = characterBody.crosshairPrefab = Resources.Load<GameObject>("Prefabs/CharacterBodies/HuntressBody").GetComponent<CharacterBody>().crosshairPrefab; //The crosshair prefab.
            characterBody.hideCrosshair = false; //Whether or not to hide the crosshair
            characterBody.hullClassification = HullClassification.Human; //The hull classification, usually used for AI
            characterBody.isChampion = false; //Set this to true if its A. A boss or B. A miniboss
            characterBody.levelArmor = 0; //Armor gained when leveling up. 
            characterBody.levelAttackSpeed = 0; //Attackspeed gained when leveling up. 
            characterBody.levelCrit = 0; //Crit chance gained when leveling up. 
            characterBody.levelDamage = 2.4f; //Damage gained when leveling up. 
            characterBody.levelArmor = 0; //Armor gained when leveling up. 
            characterBody.levelJumpPower = 0; //Jump power gained when leveling up. 
            characterBody.levelMaxHealth = 42; //Health gained when leveling up. 
            characterBody.levelMaxShield = 0; //Shield gained when leveling up. 
            characterBody.levelMoveSpeed = 0; //Move speed gained when leveling up. 
            characterBody.levelRegen = 0.5f; //Regen gained when leveling up. 
            characterBody.portraitIcon = Assets.mainAssetBundle.LoadAsset<Texture>("texHenryIcon"); //The portrait icon, shows up in multiplayer and the death UI
            characterBody.preferredPodPrefab = Resources.Load<GameObject>("prefabs/networkedobjects/robocratepod");

            LanguageAPI.Add(characterBody.baseNameToken, "Henry");
            LanguageAPI.Add(characterBody.subtitleNameToken, "The Example Above All");
        }
        #endregion
        #region Skill Creation

        private void SetSkills()
        {
            API.CreateEmptySkills(HenryBody);
            CreatePassives();
            CreatePrimary();
            CreateSecondary();
            CreateUtility();
            CreateSpecial();
            InitSkillsStates();
        }

        private void CreatePassives()
        {
            /*var passive = skillLocator.passiveSkill;

            passive.enabled = true;
            passive.skillNameToken = "HENRY_PASSIVE_NAME";
            passive.skillDescriptionToken = "HENRY_PASSIVE_DESCRIPTION";
            passive.keywordToken = "KEYWORD_VELOCITY";
            passive.icon = AssetsCore.HenryPassive;

            LanguageAPI.Add(passive.skillNameToken, "Walkman");
            LanguageAPI.Add(passive.skillDescriptionToken, "For every 3 seconds you're engaged in combat, gain a stack of <style=cIsUtility>Velocity</style>, up to 10.");

            skillLocator.passiveSkill = passive;

            LanguageAPI.Add(passive.keywordToken, "<style=cKeywordName>Velocity</style><style=cSub>Increases movement speed by X% and health regeneration by X; all stacks lost when out of combat.</style>");*/
        }

        private void CreatePrimary()
        {
            //Register our melee attack.
            LoadoutAPI.AddSkill(typeof(HenryMeleeAttack));

            SteppedSkillDef primarySkillDef = ScriptableObject.CreateInstance<SteppedSkillDef>();
            primarySkillDef.activationState = new SerializableEntityStateType(typeof(HenryMeleeAttack));
            primarySkillDef.activationStateMachineName = "Weapon";
            primarySkillDef.baseMaxStock = 1;
            primarySkillDef.baseRechargeInterval = 0f;
            primarySkillDef.beginSkillCooldownOnSkillEnd = true;
            primarySkillDef.canceledFromSprinting = false;
            primarySkillDef.fullRestockOnAssign = true;
            primarySkillDef.interruptPriority = InterruptPriority.Any;
            primarySkillDef.isBullets = false;
            primarySkillDef.isCombatSkill = true;
            primarySkillDef.mustKeyPress = false;
            primarySkillDef.noSprint = false;
            primarySkillDef.rechargeStock = 1;
            primarySkillDef.requiredStock = 1;
            primarySkillDef.shootDelay = 0.1f;
            primarySkillDef.stockToConsume = 0;
            primarySkillDef.skillDescriptionToken = "HENRY_PRIMARY_DESCRIPTION";
            primarySkillDef.skillName = "HENRY_PRIMARY_NAME";
            primarySkillDef.skillNameToken = "HENRY_PRIMARY_NAME";
            primarySkillDef.icon = Assets.mainAssetBundle.LoadAsset<Sprite>("texPrimaryIcon");
            primarySkillDef.keywordTokens = new string[] {
                 "KEYWORD_AGILE",
            };

            LanguageAPI.Add(primarySkillDef.skillNameToken, "Sword Slash");
            LanguageAPI.Add(primarySkillDef.skillDescriptionToken, "<style=cIsUtility>Agile</style>. Swing your sword of Epic Awesomeness, dealing X% damage to enemies!!"); 

            LoadoutAPI.AddSkillDef(primarySkillDef);
            SkillFamily primarySkillFamily = skillLocator.primary.skillFamily;

            primarySkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = primarySkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(primarySkillDef.skillNameToken, false, null)

            };
        }

        private void CreateSecondary()
        {
            LoadoutAPI.AddSkill(typeof(FireGun));

            SkillDef secondarySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            secondarySkillDef.activationState = new SerializableEntityStateType(typeof(FireGun));
            secondarySkillDef.activationStateMachineName = "Weapon";
            secondarySkillDef.baseMaxStock = 1;
            secondarySkillDef.baseRechargeInterval = 3f;
            secondarySkillDef.beginSkillCooldownOnSkillEnd = true;
            secondarySkillDef.canceledFromSprinting = false;
            secondarySkillDef.fullRestockOnAssign = false;
            secondarySkillDef.interruptPriority = InterruptPriority.Skill;
            secondarySkillDef.isBullets = false;
            secondarySkillDef.isCombatSkill = true;
            secondarySkillDef.mustKeyPress = false;
            secondarySkillDef.noSprint = false;
            secondarySkillDef.rechargeStock = 1;
            secondarySkillDef.requiredStock = 1;
            secondarySkillDef.shootDelay = 0.08f;
            secondarySkillDef.stockToConsume = 1;
            secondarySkillDef.skillDescriptionToken = "HENRY_SECONDARY_DESCRIPTION";
            secondarySkillDef.skillName = "aaa";
            secondarySkillDef.skillNameToken = "HENRY_SECONDARY_NAME";
            secondarySkillDef.icon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSecondaryIcon");
            secondarySkillDef.keywordTokens = new string[] {
                // "KEYWORD_AGILE",
                // "KEYWORD_CHARGEABLE",
                // "KEYWORD_CLOCKED"
             };

            LanguageAPI.Add(secondarySkillDef.skillNameToken, "Gun Power");
            LanguageAPI.Add(secondarySkillDef.skillDescriptionToken, "Shoot your gun of Epicness, dealing x% damage.");

            LoadoutAPI.AddSkillDef(secondarySkillDef);
            SkillFamily secondarySkillFamily = skillLocator.secondary.skillFamily;

            secondarySkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = secondarySkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(secondarySkillDef.skillNameToken, false, null)

            };
        }

        private void CreateUtility()
        {
            //LoadoutAPI.AddSkill(typeof(BeginOverclock));

            SkillDef utilitySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            //utilitySkillDef.activationState = new SerializableEntityStateType(typeof(DeepClean));
            utilitySkillDef.activationStateMachineName = "Weapon";
            utilitySkillDef.baseMaxStock = 1;
            utilitySkillDef.baseRechargeInterval = 5f;
            utilitySkillDef.beginSkillCooldownOnSkillEnd = true;
            utilitySkillDef.canceledFromSprinting = false;
            utilitySkillDef.fullRestockOnAssign = false;
            utilitySkillDef.interruptPriority = InterruptPriority.Skill;
            utilitySkillDef.isBullets = false;
            utilitySkillDef.isCombatSkill = true;
            utilitySkillDef.mustKeyPress = false;
            utilitySkillDef.noSprint = false;
            utilitySkillDef.rechargeStock = 1;
            utilitySkillDef.requiredStock = 1;
            utilitySkillDef.shootDelay = 0.08f;
            utilitySkillDef.stockToConsume = 1;
            utilitySkillDef.skillDescriptionToken = "HENRY_UTILITY_DESCRIPTION";
            utilitySkillDef.skillName = "aaa";
            utilitySkillDef.skillNameToken = "HENRY_UTILITY_NAME";
            utilitySkillDef.icon = Assets.mainAssetBundle.LoadAsset<Sprite>("texUtilityIcon");
            utilitySkillDef.keywordTokens = new string[] {
                 "KEYWORD_STUNNING",
                 "KEYWORD_WEIGHTLESS"
             };

            LanguageAPI.Add(utilitySkillDef.skillNameToken, "Dash Parry");
            LanguageAPI.Add(utilitySkillDef.skillDescriptionToken, "Dash forward, deflecting any projectiles that hit you.");

            LoadoutAPI.AddSkillDef(utilitySkillDef);
            SkillFamily utilitySkillFamily = skillLocator.utility.skillFamily;

            utilitySkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = utilitySkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(utilitySkillDef.skillNameToken, false, null)
            };

            /*SkillFamily.Variant variant = new SkillFamily.Variant();

            variant.skillDef = utilitySkillDef2;
            variant.unlockableName = "";

            int prevLength = utilitySkillFamily.variants.Length;
            Array.Resize<SkillFamily.Variant>(ref utilitySkillFamily.variants, prevLength + 1);
            utilitySkillFamily.variants[prevLength] = variant;*/
        }

        private void CreateSpecial()
        {
            //LoadoutAPI.AddSkill(typeof(Drone));

            SkillDef specialSkillDef = ScriptableObject.CreateInstance<SkillDef>();
            //specialSkillDef.activationState = new SerializableEntityStateType(typeof(DeployMaid));
            specialSkillDef.activationStateMachineName = "Weapon";
            specialSkillDef.baseMaxStock = 1;
            specialSkillDef.baseRechargeInterval = 3;
            specialSkillDef.beginSkillCooldownOnSkillEnd = true;
            specialSkillDef.canceledFromSprinting = false;
            specialSkillDef.fullRestockOnAssign = true;
            specialSkillDef.interruptPriority = InterruptPriority.Skill;
            specialSkillDef.isBullets = false;
            specialSkillDef.isCombatSkill = false;
            specialSkillDef.mustKeyPress = true;
            specialSkillDef.noSprint = false;
            specialSkillDef.rechargeStock = 1;
            specialSkillDef.requiredStock = 1;
            specialSkillDef.shootDelay = 0.5f;
            specialSkillDef.stockToConsume = 1;
            specialSkillDef.skillDescriptionToken = "HENRY_SPECIAL_DESCRIPTION";
            specialSkillDef.skillName = "aaa";
            specialSkillDef.skillNameToken = "HENRY_SPECIAL_NAME";
            specialSkillDef.icon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSpecialIcon");
            specialSkillDef.keywordTokens = new string[] {
                 "KEYWORD_WEIGHTLESS"
            };

            LanguageAPI.Add(specialSkillDef.skillNameToken, "Awesome Bomb");
            LanguageAPI.Add(specialSkillDef.skillDescriptionToken, "Throw an Awesome bomb, which explodes on impact!!");

            LoadoutAPI.AddSkillDef(specialSkillDef);
            SkillFamily specialSkillFamily = skillLocator.special.skillFamily;

            specialSkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = specialSkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(specialSkillDef.skillNameToken, false, null)
            };
        }

        private void InitSkillsStates()
        {
        }
        #endregion
        #region Hooks
        private void Hook()
        {
            On.RoR2.CameraRigController.OnEnable += FixFadingInLobby;

        }

        private void FixFadingInLobby(On.RoR2.CameraRigController.orig_OnEnable orig, CameraRigController rig)
        {
            if (SceneCatalog.GetSceneDefForCurrentScene().baseSceneName is "lobby") rig.enableFading = false;
            orig(rig);
        }
        #endregion


        private void CreateSurvivorDef()
        {
            string desc = "AYO HOL' UP <color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;

            LanguageAPI.Add("HENRY_DESCRIPTION", desc);
            LanguageAPI.Add("HENRY_OUTRO_FLAVOR", "...and so he left, awesome coursing through his veins");

            SurvivorDef def = new SurvivorDef()
            {
                bodyPrefab = this.HenryBody,
                descriptionToken = "HENRY_DESCRIPTION",
                displayNameToken = "HENRY_BODY_NAME",
                displayPrefab = Assets.mainAssetBundle.LoadAsset<GameObject>("HenryDisplay"),
                name = "Henry",
                outroFlavorToken = "HENRY_OUTRO_FLAVOR",
                primaryColor = Color.black,
                unlockableName = "",
            };
            SurvivorAPI.AddSurvivor(def);
        }
    
        public void Awake()
        {
            Action awake = Plugin.awake;
            if (awake == null)
            {
                return;
            }
            awake();
        }

        public void FixedUpdate()
        {
            Action fixedUpdate = Plugin.onFixedUpdate;
            if (fixedUpdate == null)
            {
                return;
            }
            fixedUpdate();
        }

        public void Start()
        {
            Action awake = Plugin.start;
            if (awake == null)
            {
                return;
            }
            awake();
        }

        public void OnEnable()
        {
            SingletonHelper.Assign<Plugin>(Plugin.instance, this);
            Log.LogI("Henry instance assigned.");
            Action awake = Plugin.onEnable;
            if (awake == null)
            {
                return;
            }
            awake();
        }

        public void OnDisable()
        {
            SingletonHelper.Unassign<Plugin>(Plugin.instance, this);
            Log.LogI("Henry instance unassigned.");
            Action awake = Plugin.onDisable;
            if (awake == null)
            {
                return;
            }
            awake();
        }
    }
}