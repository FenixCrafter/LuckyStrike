export interface User{
    username?: string,
    userId?: string,
    token?: string,
    refreshToken?: string,
    isLoggedIn?: boolean,
}

export interface Reservation{
    id?: number,
    userId?: string,
    userName?: string,
    reservationDate: Date,
    adults: number,
    children: number,
    isCanceled?: boolean,
    lanes : ReservationLane[],
    createdAt: Date,
    updatedAt: Date,
}

export interface ReservationLane{
    id?: number,
    reservationId?: number,
    laneNumber: number,
    timeframe: LaneReservationTime[],
}

export interface LaneReservationTime{
    id?: number,
    reservationLaneId?: number,
    time: number,
}