export function getAuthApiBaseUrl(environment){
    switch (environment) {
        case 'local':
            return 'https://localhost:5001/api/';
        case 'container':
            return 'http://localhost:5200/api/';
    }
}

export function getMessengerApiBaseUrl(environment){
    switch (environment) {
        case 'local':
            return 'localhost:5001';
        case 'container':
            return 'localhost:5201';
    }
}

export let basicOptions = {
    headers: {
        'Content-type': 'application/json',
        'Accept-Language': 'en-US'
    },
};

export function addAuthorizationToHeaders(authToken){
    basicOptions.headers.Authorization = `Bearer ${authToken}`;
    return basicOptions;
}

export const localEnvName = 'local';
export const containerEnvName = 'container';
