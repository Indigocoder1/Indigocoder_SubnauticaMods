class Config {
    constructor(configName, materialIndex, fileName, prefabClassID,
        rendererHierarchyPath, textureName, isVariation, variationChance, linkedConfigNames)
    {
        this.configName = configName;
        this.materialIndex = materialIndex;
        this.fileName = fileName;
        this.prefabClassID = prefabClassID;
        this.rendererHierarchyPath = rendererHierarchyPath;
        this.textureName = textureName;
        this.isVariation = isVariation;
        this.variationChance = variationChance;
        this.linkedConfigNames = linkedConfigNames;
    }

    toString = function () {
         return this.configName;
    };
}

var configList = [];
var configItemsForm = document.querySelector("#configItems");
var configNameField = document.querySelector("#configName");
var materialIndexField = document.querySelector("#materialIndex");
var fileNameField = document.querySelector("#fileName");
var prefabclassIDField = document.querySelector("#prefabclassID");
var rendererHierarchyPathField = document.querySelector("#rendererHierarchyPath");
var textureNameField = document.querySelector("#textureName");
var isVariationToggle = document.querySelector("#isVariation");
var variationChanceField = document.querySelector("#variationChance");
var linkedConfigNamesForm = document.querySelector("#names");

function downloadFile()
{
    if(configList == null)
    {
        return;
    }

    const link = document.createElement("a");
    console.log(configList);
    let content = JSON.stringify(configList, undefined, 2);
    const file = new Blob([content], { type: 'text/plain' });
    link.href = URL.createObjectURL(file);
    link.download = "ExportedConfig.json";
    link.click();
    URL.revokeObjectURL(link.href);
}

function updateConfigNameList(newConfig, adding, index = 0)
{
    if(adding)
    {
        listItem = document.createElement("option");
        listItem.text = newConfig.configName;
        listItem.value = newConfig.configName;
        linkedConfigNamesForm.appendChild(listItem);

    }
    else
    {
        listItem = linkedConfigNamesForm.childNodes[index];
        listItem.text = newConfig.configName;
        listItem.value = newConfig.configName;
    }
}

function onVariationChange()
{
    let toggleValue = isVariationToggle.checked;

    variationChanceField.disabled = !toggleValue;
}

function newConfigItem()
{
    listItem = document.createElement("option");
    listItem.text = `Config ${configItemsForm.children.length + 1}`;
    listItem.value = configItemsForm.children.length + 1;
    configItemsForm.appendChild(listItem);

    let newConfig = new Config("", null, "", "", "", "", false, -1, "");

    configList[configItemsForm.value] = newConfig;
    configItemsForm.childNodes[configItemsForm.value].innerHTML = configNameField.value;

    linkedConfigNamesForm.disabled = false;
}

function saveToConfig()
{
    let newConfig = new Config(configNameField.value, Math.round(materialIndexField.value), fileNameField.value,
    prefabclassIDField.value, rendererHierarchyPathField.value, textureNameField.value, isVariationToggle.value=="on"?false:true,
    isVariationToggle.value?variationChanceField.value:-1, "");

    let alreadyHasConfig = configList.map((config) => config.configName == configNameField.value)[0];

    configList[configItemsForm.value - 1] = newConfig;
    configItemsForm.childNodes[configItemsForm.value].innerHTML = configNameField.value;

    linkedConfigNamesForm.disabled = false;
    updateConfigNameList(newConfig, alreadyHasConfig == undefined, configItemsForm.value);
}

function loadFromConfigItem()
{
    let config = configList[configItemsForm.value - 1];

    configNameField.value = config.configName;
    materialIndexField.value = config.materialIndex;
    fileNameField.value = config.fileName;
    prefabclassIDField.value = config.prefabClassID;
    rendererHierarchyPathField.value = config.rendererHierarchyPath;
    textureNameField.value = config.textureName;
    isVariationToggle.value = config.isVariation;
    variationChanceField.value = config.variationChance;
}