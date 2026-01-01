const serverURL = '';
const buildPath = (endpoint) => {
  const clean = String(endpoint).replace(/^\/+/, '');
  return clean.toLowerCase().startsWith('api/')
    ? `/${clean}`
    : `/api/${clean}`;
};

const fetcher = async (endpoint) => {
  let payload;
  const headers = buildHeaders();
  const url = `${serverURL}${buildPath(endpoint)}`;
  console.log('GET ->', url); // debug

  try {
    const response = await fetch(url, {
      method: 'GET',
      headers,
    });

    if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);

    const text = await response.text();
    payload = text ? JSON.parse(text) : { error: 'Empty response from server' };
  } catch (err) {
    console.log(err);
    payload = { error: `Error has occurred: ${err.message}` };
  }
  return payload;
};

const poster = async (endpoint, dataToPost) => {
  let payload;
  const headers = buildHeaders();
  const url = `${serverURL}${buildPath(endpoint)}`;
  console.log('POST ->', url, dataToPost); // debug

  try {
    const response = await fetch(url, {
      method: 'POST',
      headers,
      body: JSON.stringify(dataToPost),
    });

    console.log('POST status ->', response.status); // debug

    if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);

    const text = await response.text();
    payload = text ? JSON.parse(text) : { error: 'Empty response from server' };
  } catch (error) {
    console.log(error);
    payload = { error: `Error has occurred: ${error.message}` };
  }
  return payload;
};

const buildHeaders = () => {
  const myHeaders = new Headers();
  myHeaders.append('Content-Type', 'application/json');

  const customerString = sessionStorage.getItem('customer');
  if (customerString) {
    try {
      const customer = JSON.parse(customerString);
      if (customer?.token) {
        myHeaders.append('Authorization', 'Bearer ' + customer.token);
      }
    } catch {
      // ignore JSON parse errors
    }
  }
  return myHeaders;
};

export { fetcher, poster };
