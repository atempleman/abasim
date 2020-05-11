export interface BoxScore {
    id: number;
    scheduleId: number;
    teamId: number;
    firstName: string;
    lastName: string;
    minutes: number;
    points: number;
    rebounds: number;
    assists: number;
    steals: number;
    blocks: number;
    blockedAttempts: number;
    fgm: number;
    fga: number;
    threeFGM: number;
    threeFGA: number;
    ftm: number;
    fta: number;
    oRebs: number;
    dRebs: number;
    turnovers: number;
    fouls: number;
    plusMinus: number;
}
