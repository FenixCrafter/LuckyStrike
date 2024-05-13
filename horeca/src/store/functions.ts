function getCurrentTime(): number {
    return Date.now();
}

export function isOrderOnTime(orderTime: Date): boolean {
    if (orderTime == null)
        return false;
    return Math.abs(((getCurrentTime() - orderTime.getTime()) / (1000 * 60))) < 30;
}

export function formatPrice(price: number): string {
    return new Intl.NumberFormat('nl-NL', { style: 'currency', currency: 'EUR' }).format(price);
}