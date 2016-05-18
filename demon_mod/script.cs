//Console
//IObjectText ct;
//IObject[] csl;
//string gt;
//string[] txt = new string[9];
//int j;
//
//public void Log(string str) {
//	try{
//		for (j = 0; j < csl.Length; j++) {
//			ct = (IObjectText) csl[j];
//			txt[j] = ct.GetText();
//		}
//		for (j = 1; j < csl.Length; j++) {
//	 		ct = (IObjectText) csl[j];
//	 		ct.SetText(txt[j - 1]);
//		}
//		ct = (IObjectText) csl[0];
//		ct.SetText("> " + str);
//	}
//	catch(NullReferenceException e){
//		csl = Game.GetObjectsByCustomID("Console");
//		Log(str);
//	}
//}

//Config Area
float demonStartHealth = 666f;
float demonMeleeResistance = 0.4f;
float demonProjectileResistance = 0.5f;
float demonExplosionResistance = 0.3f;
float demonFireResistance = 50f;
float demonFallResistance = 1.1f;
int demonMaxBoost = 5;
int katanaCoolDown = 25;
int demonFireCoolDown = 4000;
int demonSpawnCpuCoolDown = 5000;
int demonBoostCoolDown = 300;
int demonShootCoolDown = 3000;
int demonMinesCoolDown = 10000;
int demonTeleportCoolDown = 4000;
bool doTips = true;
int tipsInterval = 18;
bool asignTeamsToPlayers = true;
int demonMaxClones = 5;
float clonesHealth = 85f;
bool canDemonTakeFire = false;
bool canDemonPickupWeapons = false;
//End of Config Area


//SetTimer
IObjectTimerTrigger tTrigger;


//Rand
Random rand = new Random();

//Current players spawned
IPlayer[] cPlys;

//demon
IPlayer demon;
float currentHealth;
float demonLastHealth = 100f;
Vector2 lastPos;
bool alive = true;

//SendDemonStatus
IObjectText demonText = null;
Vector2 demonTextAlign = new Vector2(0f, 38f);
Color demonTexColor = new Color(230, 0, 0);
IObjectText demonTextShadow = null;
Vector2 demonTextShadowAlign = new Vector2(0.6f, 37.6f);
Color demonTexShadowColor = new Color(0, 0, 0);

//General loop
int i = 3;
IPlayer cPly;

//RandArg
string[] blerg = new string[] {
 "Arrg",
 "Grrr",
 "Ahh",
 "Haa",
 "Pff",
 "Hoo",
 "Hee",
 "Nrr",
 "Mrr",
 "xD",
 ":U"
};

//Random dead message
string[] deadMessages = new string[] {
	"!DEAD!",
	"OK ITS OVER :D",
	"REKT",
	"FATALITY",
	"AND HE DIED",
	"POOR DEMON...",
	"NOOOO",
	"CRAP",
	"HAH",
	"UH...",
	"XD",
	":U"
};

//RandChatMessage
string[] rCMSG = new string[] {
 "In fact, demons can't use human's meds.",
 "All the power of a demon comes from his katana. When he loses it, he got a new one after a few seconds.",
 "You can disable these tips messages anytime on the \"config area\" of this script.",
 "Demons can fly by jumping and blocking with his katana. But not forever!",
 "Never try to kill a demon with fire... Seriously, thats a bad idea.",
 "Demons can throw a little bunch of fire by pressing alt and blocking.",
 "These tips are generated every 18 seconds!",
 "The best way to kill a demon is with explosions and melee attack. But the melee part is quite... dangerous.",
 "Demons are not so resistent to fall damage, well.. if the default config is enabled.",
 "Don't want teams anymore? You can disable that on the config! In fact, you can custom everything...",
 "This mod was made by MrAnyone, and any typo/sugestion/idea/question, add my username on skype: i3399i",
 "Uh... Strange lady?",
 "Demons do a HUGE explosion when they die! Be careful!",
 "Demons can shot by pressing ALT + jump attacking!",
 "Legends say that demons have a chance of 1 in 300 to shoot super Bazooka missiles!",
 "Demons can clone themselves if they crouch and block (pressing alt at this time generates other effect).",
 "If the default config is enabled, demons can't be on fire!",
 "Demons can spawn a mine by crouching and attacking.",
 "Holding ALT + crouching attacking makes the demon teleport in a random place of the map!",
 "There are some easter eggs in this mod...",
 "If the demon is dead, his clones loses power and explode after a few seconds.",
 "Demons can't pickup fire weapons! If the default config is enabled..."
};

//Version
string ver = "3.2";

//Spawn list
IObject[] spawners;

//Demon Movement
int boostTimes = 0;
int lastBoostMilisec = Environment.TickCount;
int lastMessageMilisec = Environment.TickCount;
int lastFireMilisec = Environment.TickCount;
int lastJumpAttackMilisec = Environment.TickCount;
int lastCpuMilisec = Environment.TickCount;
int lastTeleportMilisec;
int lastMinesMilisec;

//DisplayHealth
bool didIt = false;

//SpawnCpu
int cpusCount = 0;
Vector2 cVec;

//DeathTrigger
IObjectOnPlayerDeathTrigger dTrigger;
IUser cUser;

//Pause demon movement on startup
IObjectTimerTrigger pauseDemon;
Vector2 lastSpawnPos;

//Remove Weapons Tick
WeaponItem demonRifle;
WeaponItem demonHandGun;
WeaponItem demonThrownWeapon;

public IObjectTimerTrigger SetTimer(string met, string id, int times, int delay){
	tTrigger = (IObjectTimerTrigger)Game.CreateObject("TimerTrigger");
	tTrigger.CustomId = id;
	tTrigger.SetScriptMethod(met);
	tTrigger.SetIntervalTime(delay);
	tTrigger.SetRepeatCount(times);
	tTrigger.Trigger();
	return tTrigger;
}

public void OnStartup() {
	cPlys = Game.GetPlayers();
	spawners = Game.GetObjectsByName("SpawnPlayer");
	demon = ChooseDemon();
	dTrigger = (IObjectOnPlayerDeathTrigger)Game.CreateObject("OnPlayerDeathTrigger");
	dTrigger.SetScriptMethod("OnDeath");
	pauseDemon = SetTimer("PauseDemon", "", 0, 100);
	if(asignTeamsToPlayers){
		foreach(IPlayer Player in cPlys){
			if(!Player.IsBot){
				if(Player == demon)
					Player.SetTeam(PlayerTeam.Team2);
				else
					Player.SetTeam(PlayerTeam.Team3);
			}
		}
	}
	SetDemon();
	SetTimer("StartDemon", "", i + 2, 1000);
	Game.RunCommand("/INFINITE_LIFE 1");
	SendDemonStatus("Oh... strange lady?");
	SendMessage(": Welcome to Demon Lord Mod! First time? There's a lot of moves that demons can do! Discover them!");
	SendMessage("Version: " + ver);
	SendMessage(": Made by MrAnyone");
}

public IPlayer ChooseDemon() {
	return cPlys[rand.Next(0, cPlys.Length)];
}

//Starts the demon on countdown
public void StartDemon(TriggerArgs args){
	if(i >= 0){
		lastSpawnPos = spawners[rand.Next(0, spawners.Length)].GetWorldPosition();
		demon.SetWorldPosition(lastSpawnPos);
		demon.SetInputEnabled(false);
		PlayDemonTextEffect(i.ToString());
		SendDemonStatus(i.ToString() + " - Demon - " + i.ToString());
		i--;
	}
	else {
		demon.SetInputEnabled(true);
		SetTimer("DemonLifeTick", "", 0, 10);
		SetTimer("DemonHealthDisplayTick", "DHDT", 0, 50);
		SetTimer("DemonMovementTick", "", 0, 100);
		SetTimer("RandChatMessage", "", 0, 1000 * tipsInterval);
		SetTimer("CheckKatana", "", 0, 1000 * katanaCoolDown);
		SetTimer("RemoveWeaponsTick", "", 0, 20);
		CheckKatana(args);
		Game.RunCommand("/INFINITE_LIFE 0");
		pauseDemon.Destroy();
	}
}

public void SetDemon(){
}

public void CheckKatana(TriggerArgs args){
	if(!(demon.CurrentMeleeWeapon.WeaponItem == WeaponItem.KATANA) && !demon.IsDead){
		demon.GiveWeaponItem(WeaponItem.KATANA);
		PlayDemonTextEffect("Haha now I have power!");
	}
}

public void DemonLifeTick(TriggerArgs args){
	if(alive){
		//Damage
		currentHealth = (demonStartHealth + 100) - (demon.Statistics.TotalMeleeDamageTaken / demonMeleeResistance +
				demon.Statistics.TotalProjectileDamageTaken / demonProjectileResistance +
				demon.Statistics.TotalExplosionDamageTaken / demonExplosionResistance +
				demon.Statistics.TotalFireDamageTaken / demonFireResistance +
				demon.Statistics.TotalFallDamageTaken / demonFallResistance);
		
		demon.SetHealth(currentHealth);

		if(!canDemonTakeFire && demon.IsBurning){
			demon.ClearFire();
		}
	
			//Kill
			if(demon.GetHealth() < demonLastHealth){
				demon.Kill();
				alive = false;
		}
	}
}

public void DemonMovementTick(TriggerArgs args){
	if(!demon.IsDead){
		if(demon.CurrentWeaponDrawn == WeaponItemType.Melee && demon.CurrentMeleeWeapon.WeaponItem == WeaponItem.KATANA){
			if(demon.IsBlocking && demon.IsCrouching && demon.IsWalking && (lastCpuMilisec + demonSpawnCpuCoolDown) < Environment.TickCount){
				if(cpusCount < demonMaxClones){
					SpawnCpu(0);
					lastCpuMilisec = Environment.TickCount;
				}
				else{
					PlayDemonTextEffect("Too many clones!");
				}
			}
			else if(demon.IsBlocking && demon.IsCrouching && !demon.IsWalking && (lastCpuMilisec + demonSpawnCpuCoolDown) < Environment.TickCount){
				if(cpusCount < demonMaxClones){
					SpawnCpu(1);
					lastCpuMilisec = Environment.TickCount;
				}
				else{
					PlayDemonTextEffect("Too many clones!");
				}
			}
			else if(demon.IsBlocking && demon.IsWalking && (lastFireMilisec + demonFireCoolDown) < Environment.TickCount && !demon.IsCrouching){
					Game.SpawnFireNodes(
						demon.GetWorldPosition() + new Vector2(demon.FacingDirection * 10, 10),
						rand.Next(3, 10),
						new Vector2(demon.FacingDirection * 6, 0),
						1,
						2,
						FireNodeType.Flamethrower);
					lastFireMilisec = Environment.TickCount;
					PlayDemonTextEffect("Fire!");
					PlayDemonSound("FlameThrower", 100f);
			}
			if(demon.IsBlocking && boostTimes < demonMaxBoost && (lastBoostMilisec + demonBoostCoolDown) < Environment.TickCount && !demon.IsOnGround){
				boostTimes++;
				PlayDemonSound("FlareGun", 10f);
				demon.SetLinearVelocity(new Vector2(demon.FacingDirection*2f,10f));
				PlayDemonTextEffect(RandArg());
				lastBoostMilisec = Environment.TickCount;
			}
	
	
			if(demon.IsJumpAttacking && (lastJumpAttackMilisec + demonShootCoolDown) < Environment.TickCount && !demon.IsOnGround && demon.IsWalking){
				if(rand.Next(0, 300) == 151){
					FireWeapon(99);
				}
				else
					FireWeapon(rand.Next(0, 4));
				lastJumpAttackMilisec = Environment.TickCount;
				PlayDemonTextEffect(RandArg());
			}
			//else if(demon.IsJumpAttacking && (lastJumpAttackMilisec + 3000) < Environment.TickCount && !demon.IsOnGround){
			//}
			if (demon.IsCrouching && demon.IsMeleeAttacking && (lastMinesMilisec + demonMinesCoolDown) < Environment.TickCount && !demon.IsWalking){
				lastMinesMilisec = Environment.TickCount;
				Game.CreateObject("WpnMineThrown", lastPos, 0f , new Vector2(3f * demon.FacingDirection, 4f), 2f);
			}
			else if(demon.IsCrouching && demon.IsMeleeAttacking && (lastTeleportMilisec + demonTeleportCoolDown) < Environment.TickCount && demon.IsWalking){
				lastTeleportMilisec = Environment.TickCount;
				cVec = spawners[rand.Next(0, spawners.Length)].GetWorldPosition();
				Game.PlayEffect("Electric", cVec);
				demon.SetWorldPosition(cVec);
			}

			if(demon.IsOnGround)
				boostTimes = 0;
		}
		else if((lastMessageMilisec + 3000) < Environment.TickCount){
			lastMessageMilisec = Environment.TickCount;
			PlayDemonTextEffect("No KATANA! ARRG");
		}
		lastPos = demon.GetWorldPosition();
	}
}

public void DemonHealthDisplayTick(TriggerArgs args){
	if(!demon.IsDead)
		SendDemonStatus(((int)currentHealth - (int)demonLastHealth).ToString());
	else{
		SendDemonStatus(deadMessages[rand.Next(0, deadMessages.Length)]);
		Game.GetSingleObjectByCustomID("DHDT").Destroy();
		SetTimer("GibCpus", "", 1, 5000);
	}
}

public void SendDemonStatus(string msg){
	if(demonText == null && demonTextShadow == null){
		demonTextShadow = (IObjectText)Game.CreateObject("Text", demon.GetWorldPosition() + demonTextShadowAlign, 0f);
		demonTextShadow.SetTextAlignment(TextAlignment.Middle);
		demonTextShadow.SetTextScale(0.8f);
		demonTextShadow.SetTextColor(demonTexShadowColor);

		demonText = (IObjectText)Game.CreateObject("Text", demon.GetWorldPosition() + demonTextAlign, 0f);
		demonText.SetTextAlignment(TextAlignment.Middle);
		demonText.SetTextScale(0.8f);
		demonText.SetTextColor(demonTexColor);
	}
	if(!demon.IsDead){
		demonText.SetWorldPosition(demon.GetWorldPosition() + demonTextAlign);
		demonTextShadow.SetWorldPosition(demon.GetWorldPosition() + demonTextShadowAlign);
	}
	else{
		demonText.SetWorldPosition(lastPos + demonTextAlign);
		demonTextShadow.SetWorldPosition(lastPos + demonTextShadowAlign);
	}
	demonText.SetText("[" + msg + "]");
	demonTextShadow.SetText("[" + msg + "]");
}

public void PlayDemonSound(string what, float vol){
	Game.PlaySound(what, demon.GetWorldPosition(), vol);
}

public void SendMessage(string msg){
	Game.RunCommand("/MSG |DEMON LORD MOD| " + msg);
}

public void PlayDemonTextEffect(string text){
	if(!demon.IsDead)
		Game.PlayEffect("CFTXT", demon.GetWorldPosition() + new Vector2(3f * demon.FacingDirection, 10f), text);
	else
		Game.PlayEffect("CFTXT", lastPos + new Vector2(3f * demon.FacingDirection, 10f), text);
}

public string RandArg(){
	return blerg[rand.Next(0, blerg.Length)];
}

public void RandChatMessage(TriggerArgs args){
	if(doTips)
		SendMessage("Tip: " + rCMSG[rand.Next(0, rCMSG.Length)]);
}

public void FireWeapon(int id){
	switch(id){
		case 0:
			PlayDemonSound("Sniper", 100f);
			Game.SpawnProjectile(ProjectileItem.SNIPER, demon.GetWorldPosition() + new Vector2(8f * demon.FacingDirection, 10f), new Vector2(demon.FacingDirection , (float)rand.Next(-5, 5)/1000));
			for(int i = 0; i < 15; i++){				
				Game.PlayEffect("PLRB", demon.GetWorldPosition() + new Vector2(15f * demon.FacingDirection + rand.Next(-8, 8), rand.Next(5, 9)));
			}
			break;
		case 1:
			PlayDemonSound("Magnum", 100f);
			Game.SpawnProjectile(ProjectileItem.MAGNUM, demon.GetWorldPosition() + new Vector2(8f * demon.FacingDirection, 10f), new Vector2(demon.FacingDirection , (float)rand.Next(-30, 30)/1000));
			for(int i = 0; i < 5; i++){
				Game.PlayEffect("FIRE", demon.GetWorldPosition() + new Vector2(15f * demon.FacingDirection + rand.Next(-8, 8), rand.Next(5, 9)));
			}
			break;
		case 2:
			PlayDemonSound("Revolver", 100f);
			Game.SpawnProjectile(ProjectileItem.REVOLVER, demon.GetWorldPosition() + new Vector2(8f * demon.FacingDirection, 10f), new Vector2(demon.FacingDirection , (float)rand.Next(-40, 40)/1000));
			for(int i = 0; i < 5; i++){
				Game.PlayEffect("STM", demon.GetWorldPosition() + new Vector2(15f * demon.FacingDirection + rand.Next(-10, 10), rand.Next(1, 9)));
			}
			break;
		case 3:
			PlayDemonSound("Shotgun", 100f);
			for(int i = 0; i < 13; i++){
				Game.SpawnProjectile(ProjectileItem.SHOTGUN, demon.GetWorldPosition() + new Vector2(8f * demon.FacingDirection, 10f), new Vector2(demon.FacingDirection , (float)rand.Next(-100, 100)/1000));
			}
			Game.PlayEffect("ACS", demon.GetWorldPosition() + new Vector2(15f * demon.FacingDirection + rand.Next(-10, 10), rand.Next(1, 9)));
			break;
		case 99:
			for(int i = 0; i < 4; i++){
				PlayDemonSound("Bazooka", 100f);
				Game.SpawnProjectile(ProjectileItem.BAZOOKA, demon.GetWorldPosition() + new Vector2(8f * demon.FacingDirection, 10f), new Vector2(demon.FacingDirection , (float)rand.Next(-100, 100)/1000));
			}
			for(int i = 0; i < 30; i++){				
				Game.PlayEffect("PLRB", demon.GetWorldPosition() + new Vector2(15f * demon.FacingDirection + rand.Next(-15, 15), rand.Next(2, 13)));
			}
			break;
	}
}

public void SpawnCpu(int mode){
	switch(mode){
		case 0:
			cVec = spawners[rand.Next(0, spawners.Length)].GetWorldPosition();
			break;
		default:
			cVec = lastPos;
			break;
	}

	cPly = Game.CreatePlayer(cVec);
	cPly.SetTeam(demon.GetTeam());
	cPly.SetHealth(clonesHealth);
	cPly.SetBotType(BotType.TutorialA);
	cPly.SetProfile(demon.GetProfile());
	cpusCount++;
	Game.PlayEffect("Electric", cVec);
}

public void OnDeath(TriggerArgs args){
	cPly = (IPlayer)args.Sender;
	if(cPly.IsBot && cPly.GetTeam() == demon.GetTeam()){
		Game.PlayEffect("CFTXT", cPly.GetWorldPosition(), RandArg());
		cpusCount--;
	}
	if(cPly == demon){
			lastPos = cPly.GetWorldPosition();
			for(i = 0; i < 10; i++){
			Game.TriggerExplosion(lastPos);
			Game.SpawnFireNodes(
					lastPos,
					rand.Next(10, 50),
					new Vector2(rand.Next(-10, 10), rand.Next(0, 20)),
					1,
					5,
					FireNodeType.Flamethrower);
			}
			if(rand.Next(0, 3) >= 2){
				PlayDemonSound("Wilhelm", 100f);
		}
	}
}

//Pauses the demon on startup
public void PauseDemon(TriggerArgs args){
	demon.SetWorldPosition(lastSpawnPos);
	demon.SetLinearVelocity(Vector2.Zero);
}

public void GibCpus(TriggerArgs args){
	foreach(IPlayer ply in Game.GetPlayers()){
		if(ply.IsBot){
			ply.Gib();
		}
	}
}

public void RemoveWeaponsTick(TriggerArgs args){
	if(!canDemonPickupWeapons){
		if(demon.CurrentPrimaryWeapon.WeaponItemType != WeaponItemType.NONE){
			demonRifle = demon.CurrentPrimaryWeapon.WeaponItem;
			demon.RemoveWeaponItemType(WeaponItemType.Rifle);
			Game.SpawnWeaponItem(demonRifle, lastPos);
		}
		if(demon.CurrentSecondaryWeapon.WeaponItemType != WeaponItemType.NONE){
			demonHandGun = demon.CurrentSecondaryWeapon.WeaponItem;
			demon.RemoveWeaponItemType(WeaponItemType.Handgun);
			Game.SpawnWeaponItem(demonHandGun, lastPos);
		}
		if(demon.CurrentThrownItem.WeaponItemType != WeaponItemType.NONE){
			demonThrownWeapon = demon.CurrentThrownItem.WeaponItem;
			demon.RemoveWeaponItemType(WeaponItemType.Thrown);
			Game.SpawnWeaponItem(demonThrownWeapon, lastPos);
		}
	}
}