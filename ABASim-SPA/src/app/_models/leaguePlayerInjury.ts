export interface LeaguePlayerInjury {
    playerId: number;
    playerName: string;
    teamName: string;
    severity: number;
    type: string;
    timeMissed: number;
    startDay: number;
    endDay: number;
}
