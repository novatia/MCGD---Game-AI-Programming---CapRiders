using System;

namespace HutongGames.PlayMaker.Actions
{
    [Serializable]
    public class tnCharacterEntry
    {
        public FsmString characterId = new FsmString { UseVariable = false, Value = "NULL" };
        public FsmInt onlinePlayerIndex = new FsmInt { UseVariable = false, Value = 0 };
        public FsmString playerId = new FsmString { UseVariable = false, Value = "NULL" };
        public FsmInt spawnOrder = new FsmInt { UseVariable = false, Value = -1 };
    }

    [ActionCategory("TuesdayNights")]
    [Tooltip("Add a new team to TeamsModule.")]
    public class tnAddTeamToModule : FsmStateAction
    {
        [RequiredField]
        public FsmString id;

        [RequiredField]
        public FsmColor color;

        [RequiredField]
        public FsmString captainId;
        [RequiredField]
        public tnCharacterEntry[] characters;

        public override void OnEnter()
        {
            tnTeamsModule teamsModule = GameModulesManager.GetModuleMain<tnTeamsModule>();

            if (teamsModule == null)
            {
                teamsModule = GameModulesManager.AddModuleMain<tnTeamsModule>();
                teamsModule.Clear();
            }

            tnTeamDescription team = new tnTeamDescription();

            team.SetTeamId(id.Value);
            team.SetTeamColor(color.Value);

            int charactersCount = characters.Length;
            for (int characterIndex = 0; characterIndex < charactersCount; ++characterIndex)
            {
                tnCharacterEntry entry = characters[characterIndex];
                if (entry != null)
                {
                    FsmString characterId = entry.characterId;
                    FsmInt onlinePlayerIndex = entry.onlinePlayerIndex;
                    FsmString playerId = entry.playerId;
                    FsmInt spawnOrder = entry.spawnOrder;

                    if (characterId != null && !characterId.IsNone && onlinePlayerIndex != null && !onlinePlayerIndex.IsNone && playerId != null && !playerId.IsNone && spawnOrder != null && !spawnOrder.IsNone)
                    {
                        tnCharacterDescription character = new tnCharacterDescription();

                        character.SetCharacterId(characterId.Value);

                        character.SetOnlinePlayerIndex(onlinePlayerIndex.Value);

                        character.SetPlayerId(playerId.Value);
                        character.SetSpawnOrder(spawnOrder.Value);

                        team.AddCharacterDescription(character);
                    }
                }
            }

            teamsModule.AddTeamDescription(team);

            Finish();
        }
    }
}

