using System.Collections.Generic;


public static class StaticInfo 
{
    public static CharacterBasic firstCharcter = new CharacterBasic();
    public static CharacterBasic characterToAcces=new CharacterBasic();
    public static List<CharacterBasic> teamSelectedToPlay;
    public static List<CharacterBasic> rivalTeam;
    public static TeamTournamentInfo tournamentTeam =null;

    public static int previousScene;

}
