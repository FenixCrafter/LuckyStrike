import type { Product } from "!/ApiClient";

export interface ProductOrder {
    product: Product,
    amount: number
}

export interface Order {
    id: number,
    readyToDeliver: boolean,
    lane: number,
    timeOrdered: Date,
    inProgress: boolean,
    isLate: boolean
}

export interface OrderItems {
    [category: string]: {
        name: string,
        amount: number,
        finished: boolean
    }[]
}

export interface User{
    username: string;
    userId: string;
    token: string;
    refreshToken: string,
    isLoggedIn: boolean;
}