
const hostAdress = process.env.NODE_ENV === "production" ? "" : "http://localhost:5000";

function buildUrl(url, queryStringParameters) {
    const queryString = queryStringParameters?.map(param => `${param.name}=${param.value}`).join('&');
    return `${hostAdress}/${url}${queryString ? `?${queryString}` : ""}`;
}

export async function httpGet(url, queryStringParameters, deserialize = true) {
    return await sendRequest(url, queryStringParameters, null, deserialize);
}

export async function httpPut(url, queryStringParameters, payload, deserialize = true) {
    const requestOptions = {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    };

    return await sendRequest(url, queryStringParameters, requestOptions, deserialize);
}

export async function httpDelete(url, queryStringParameters, payload, deserialize = true) {
    const requestOptions = {
        method: 'DELETE',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    };

    return await sendRequest(url, queryStringParameters, requestOptions, deserialize);
}

async function sendRequest(url, queryStringParameters, requestOptions, deserialize) {
    const response = await fetch(buildUrl(url, queryStringParameters), requestOptions);
    if (response.ok) {
        try {
            const result = deserialize ? await response.json() : response.statusText;

            return result;
        }
        catch {
            return null;
        }
    }
    else
        return null;
}