import assert from 'node:assert/strict';
import { createHash } from 'node:crypto';
import { readFileSync } from 'node:fs';
import test from 'node:test';
import vm from 'node:vm';

const expectedSha256 =
  '6FDD5849C76354BDAF64C06DD4A9ED0F8ED0BBBF8D42E58DDDC7789E05271B64';
const vendorPath = new URL(
  '../../src/Web/vendor/ba-click-fx.iife.js',
  import.meta.url,
);
const adapterPath = new URL('../../src/Web/fx-adapter.js', import.meta.url);
const templatePath = new URL('../../src/Web/index.html', import.meta.url);

test('vendored artifact matches the reviewed v1.2.11 build', () =>
{
  const bytes = readFileSync(vendorPath);
  const actual = createHash('sha256').update(bytes).digest('hex').toUpperCase();

  assert.equal(actual, expectedSha256);
});

test('vendored IIFE exposes every host API required by BASpark', () =>
{
  const source = readFileSync(vendorPath, 'utf8');
  const context = {};

  vm.runInNewContext(source, context, { filename: 'ba-click-fx.iife.js' });

  assert.equal(typeof context.BAClickFX.BAClickFX, 'function');
  assert.equal(context.BAClickFX.BLOOM_BACKEND_CHANGE_EVENT, 'baclickfxbackendchange');

  const prototype = context.BAClickFX.BAClickFX.prototype;
  for (const method of [
    'pointerDown',
    'pointerMove',
    'pointerUp',
    'pointerCancel',
    'clearTrail',
    'setPaused',
    'updateConfig',
    'setThemeColor',
    'destroy',
  ])
  {
    assert.equal(typeof prototype[method], 'function', `${method} must remain public`);
  }
});

test('inline resources cannot terminate their script element early', () =>
{
  const vendor = readFileSync(vendorPath, 'utf8');
  const adapter = readFileSync(adapterPath, 'utf8');

  assert.equal(/<\/script/i.test(vendor), false);
  assert.equal(/<\/script/i.test(adapter), false);
  assert.equal(vendor.includes('sourceMappingURL'), false);
});

test('renderer template contains one deterministic injection marker', () =>
{
  const template = readFileSync(templatePath, 'utf8');
  const marker = '<!-- BASPARK_RENDERER_SCRIPTS -->';

  assert.equal(template.split(marker).length - 1, 1);
});
