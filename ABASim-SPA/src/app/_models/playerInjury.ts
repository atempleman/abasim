export interface PlayerInjury {
    id: number;
    playerId: number;
    severity: number;
    type: string;
    timeMissed: number;
    startDay: number;
    endDay: number;
    currentlyInjured: number;
}
