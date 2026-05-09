import test from 'node:test';
import assert from 'node:assert/strict';
import { formatCurrency, formatLocalDateTime } from '../src/utils/formatutils.js';
import { fetcher, poster } from '../src/utils/apiutil.js';

const installSessionStorage = (value) => {
  globalThis.sessionStorage = {
    getItem: (key) => (key === 'customer' ? value : null),
  };
};

test('formatCurrency renders USD currency with cents', () => {
  assert.equal(formatCurrency(1234.5), '$1,234.50');
});

test('formatLocalDateTime returns original value when date parsing fails', () => {
  assert.equal(formatLocalDateTime('not-a-date'), 'not-a-date');
});

test('fetcher prefixes API paths and forwards bearer token', async () => {
  installSessionStorage(JSON.stringify({ token: 'abc123' }));

  let request;
  globalThis.fetch = async (url, options) => {
    request = { url, options };
    return {
      ok: true,
      text: async () => JSON.stringify([{ id: 1, name: 'Citrus' }]),
    };
  };

  const payload = await fetcher('brands');

  assert.deepEqual(payload, [{ id: 1, name: 'Citrus' }]);
  assert.equal(request.url, '/api/brands');
  assert.equal(request.options.method, 'GET');
  assert.equal(request.options.headers.get('Authorization'), 'Bearer abc123');
});

test('poster preserves existing api prefix and sends JSON body', async () => {
  installSessionStorage(null);

  let request;
  globalThis.fetch = async (url, options) => {
    request = { url, options };
    return {
      ok: true,
      status: 201,
      text: async () => JSON.stringify({ saved: true }),
    };
  };

  const payload = await poster('/api/customer/register', { email: 'buyer@test.com' });

  assert.deepEqual(payload, { saved: true });
  assert.equal(request.url, '/api/customer/register');
  assert.equal(request.options.method, 'POST');
  assert.equal(request.options.headers.get('Content-Type'), 'application/json');
  assert.equal(request.options.body, JSON.stringify({ email: 'buyer@test.com' }));
});

test('fetcher returns an error object for non-OK responses', async () => {
  installSessionStorage(null);

  globalThis.fetch = async () => ({
    ok: false,
    status: 500,
    text: async () => 'server error',
  });

  const payload = await fetcher('products');

  assert.match(payload.error, /HTTP error! status: 500/);
});
