const path = require("path");
var JSZip = require("jszip");
const fs = require("fs-extra");

const projectName = "ValheimTwitch";
const buildDir = path.resolve(__dirname, "../build");
const pluginFile = path.resolve(__dirname, "..", projectName, "Plugin.cs");

const pluginSource = fs.readFileSync(pluginFile);

const matches = pluginSource
  .toString()
  .match(/VERSION = "([0-9]+\.[0-9]+\.[0-9]+)"/);

const projectVersion = matches[1];

console.log(`>>> Build v${projectVersion}`);

const dllName = `${projectName}.dll`;
const inputDLL = path.join(buildDir, dllName);
const templateDir = path.join(__dirname, "build-template");
const outputDir = path.join(buildDir, `${projectName}-v${projectVersion}`);

const manifest = require(`${__dirname}/manifest.json`);

manifest.version_number = projectVersion;

fs.removeSync(outputDir);

if (!fs.existsSync(inputDLL)) {
  console.log("[ERROR] DLL not found, please rebuild from Visual Studio!!!");
  process.exit(1);
}

fs.copySync(templateDir, outputDir);
fs.moveSync(inputDLL, path.join(outputDir, dllName));

const readmepath = path.resolve(__dirname, "../README.md");
const outputDLL = path.join(outputDir, dllName);
const dllBuffer = fs.readFileSync(outputDLL);

async function updateZIP(zipname, dllpath, content) {
  const filepath = path.join(outputDir, zipname);
  const zip = await JSZip.loadAsync(fs.readFileSync(filepath));
  const file = await zip.file(dllpath, content).generateAsync({
    type: "nodebuffer",
    platform: process.platform,
  });
  fs.writeFileSync(filepath, file);
}

(async () => {
  await updateZIP(
    "BepInExPack_ValheimTwitch.zip",
    "BepInEx/plugins/ValheimTwitch/ValheimTwitch.dll",
    dllBuffer
  );
  await updateZIP(
    "Thunderstore.zip",
    "BepInEx/plugins/ValheimTwitch/ValheimTwitch.dll",
    dllBuffer
  );
  await updateZIP("Thunderstore.zip", "README.md", fs.readFileSync(readmepath));
  await updateZIP(
    "Thunderstore.zip",
    "manifest.json",
    JSON.stringify(manifest, null, 2)
  );
  await updateZIP(
    "Vortex.zip",
    "plugins/ValheimTwitch/ValheimTwitch.dll",
    dllBuffer
  );
  console.log("Done!");
})();
