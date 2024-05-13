// zet de payload van de token om naar een JSON object met data
export function getPayloadFromToken(token: string): any {
    return JSON.parse(window.atob(token.split('.')[1]));
}

export function getRolesFromToken(token: string): string[] {
    let roles: string[] = getPayloadFromToken(token).role;
    if (roles == null)
        return [];
    if (typeof(roles) == "string") {
        roles = [roles];
    }
    return roles
}

export function roleMatch(roles: string[]): boolean {
    const token = localStorage.getItem("Token");
    if (token == null || token == '' || token == undefined || token == "undefined")
        return false;
    
    let userRoles = getRolesFromToken(token);
    
    let isAllowed = false;

    roles.forEach(role => {
        isAllowed = userRoles.includes(role) || isAllowed;
    });
    return isAllowed;
}