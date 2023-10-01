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

window.onbeforeunload = function (e)
{
    return "Are you sure?";
}

var configList = [];
var configItemsForm = document.getElementById("configItems");
var configNameField = document.getElementById("configName");
var materialIndexField = document.getElementById("materialIndex");
var fileNameField = document.getElementById("fileName");
var prefabclassIDField = document.getElementById("prefabclassID");
var rendererHierarchyPathField = document.getElementById("rendererHierarchyPath");
var textureNameField = document.getElementById("textureName");
var isVariationToggle = document.getElementById("isVariation");
var variationChanceField = document.getElementById("variationChance");
var linkedConfigNamesForm = document.getElementById("names");

var triedToDelete;

function downloadFile()
{
    if(configList == null)
    {
        return;
    }

    const link = document.createElement("a");
    console.log(configList);
    var content = JSON.stringify(configList, undefined, 2);
    const file = new Blob([content], { type: 'text/plain' });
    link.href = URL.createObjectURL(file);
    link.download = "ExportedConfig.json";
    link.click();
    URL.revokeObjectURL(link.href);
}

function updateConfigNameList(newConfig, adding, index = 0)
{
    if(adding && !triedToDelete)
    {
        listItem = document.createElement("option");
        listItem.text = newConfig.configName;
        listItem.value = newConfig.configName;
        linkedConfigNamesForm.appendChild(listItem);

    }
    else
    {
        listItem = linkedConfigNamesForm.options[index];
        listItem.text = newConfig.configName;
        listItem.value = newConfig.configName;
    }

    triedToDelete = false;
}

function removeLinkedConfigName(linkedConfigName)
{
    if(!linkedConfigNamesForm)
    {
        return;
    }

    for(let i = 0; i < linkedConfigNamesForm.length; i++)
    {
        if(linkedConfigNamesForm[i].value == linkedConfigName)
        {
            linkedConfigNamesForm[i].remove();
        }
    }
}

function onVariationChange()
{
    var toggleValue = isVariationToggle.checked;

    variationChanceField.disabled = !toggleValue;
}

function newConfigItem()
{
    listItem = document.createElement("option");
    listItem.text = `Config ${configItemsForm.children.length + 1}`;
    listItem.value = configItemsForm.children.length + 1;
    configItemsForm.appendChild(listItem);

    var newConfig = new Config("", null, "", "", "", "", false, -1, "");

    configList[configItemsForm.value] = newConfig;

    linkedConfigNamesForm.disabled = false;
}

function saveToConfig()
{
    var linkedConfigsArray = [];
    var options = linkedConfigNamesForm.options;
    let currentArrayIndex = 0;
    for(let i = 0; i < options.length; i++)
    {
       if(options[i].selected)
       {
            linkedConfigsArray[currentArrayIndex] = options[i].value;
            currentArrayIndex++;
       }
    }

    var newConfig = new Config(configNameField.value, Math.round(materialIndexField.value), fileNameField.value,
    prefabclassIDField.value, rendererHierarchyPathField.value, textureNameField.value, isVariationToggle.value=="on"?false:true,
    isVariationToggle.value?variationChanceField.value:-1, linkedConfigsArray);

    var alreadyHasConfig = configList.map((config) => config.configName == configNameField.value).includes(true);

    configList[configItemsForm.value - 1] = newConfig;

    var index = configItemsForm.value - 1;
    if(configItemsForm.options[index].innerText != configNameField.value)
    {
        removeLinkedConfigName(configItemsForm.options[index].innerText);
    }

    console.log(triedToDelete);

    configItemsForm.options[index].innerText = configNameField.value;

    linkedConfigNamesForm.disabled = false;
    updateConfigNameList(newConfig, alreadyHasConfig == undefined || alreadyHasConfig == false, index);
    alert("Config saved!");
}

function deleteConfigItem()
{
    var index = configItemsForm.value - 1;
    if(index != 0)
    {
        configItemsForm.options[index].remove();
        linkedConfigNamesForm.options[index].remove();
    }
    else
    {
        alert("You need at least one config!");
        triedToDelete = true;
    }
    
    configList.splice(index, 1);
}

function loadFromConfigItem()
{
    var config = configList[configItemsForm.value - 1];

    configNameField.value = config.configName;
    materialIndexField.value = config.materialIndex;
    fileNameField.value = config.fileName;
    prefabclassIDField.value = config.prefabClassID;
    rendererHierarchyPathField.value = config.rendererHierarchyPath;
    textureNameField.value = config.textureName;
    isVariationToggle.value = config.isVariation;
    variationChanceField.value = config.variationChance;
}