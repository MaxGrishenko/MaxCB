// When Edit, we need create and fill dynamically created fields


function AddTypes(Ing, Met, Tip) {

    Ing = JSON.parse(Ing);
    Met = JSON.parse(Met);
    Tip = JSON.parse(Tip);
    for (let i = 1; i < Ing.length; i++) {
        AddType('ingredient', Ing[i], '');
    }
    for (let i = 0; i < Met.length; i++) {
        if (!i) {
            SetTextArea(Met[i], "m0");
        }
        else {
            AddType('method', Met[i], i.toString());
        }
    }
    for (let i = 0; i < Tip.length; i++) {
        if (!i) {
            SetTextArea(Tip[i], "t0");
        }
        else {
            AddType('tip', Tip[i], i.toString());
        }
    }
}

function AddType(type, text, id) {
    const division = document.createElement("DIV");
    switch (type) {
        case 'ingredient': division.innerHTML = '<div><hr/><input class="form-control" asp-for=Ingredients" name="Ingredients" type="text"  value="' + text + '" placeholder="Введите новый ингредиент"/> <input type="button" value="Удалить ингредиент" onclick="DeleteType(this, `ingredient`)" class="form-control btn btn-outline-danger" /></div>';
            document.getElementById("IngredientDiv").appendChild(division);
            break;
        case 'method': division.innerHTML = '<div><hr/><textarea id="m' + id + '" class="form-control" asp-for="Methods" name="Methods" placeholder="Опишите новый шаг"></textarea> <input type="button" value="Удалить шаг" onclick="DeleteType(this, `method`)" class="form-control btn btn-outline-danger"  /></div>';
            document.getElementById("MethodDiv").appendChild(division);
            SetTextArea(text, "m" + id)
            break;
        case 'tip': division.innerHTML = '<div><hr/><textarea id="t' + id + '" class="form-control" asp-for="Tips" name="Tips" placeholder="Расскажите о новой подсказке"></textarea > <input type="button" value="Удалить шаг" onclick="DeleteType(this, `tip`)" class="form-control btn btn-outline-danger" /></div>';
            document.getElementById("TipDiv").appendChild(division);
            SetTextArea(text, "t" + id)
            break;
    }
}

function SetTextArea(text, id) {
    document.getElementById(id).innerHTML = text;
}

function DeleteType(div, type) {
    switch (type) {
        case 'ingredient': document.getElementById("IngredientDiv").removeChild(div.parentNode.parentNode);
            break;
        case 'method': document.getElementById("MethodDiv").removeChild(div.parentNode.parentNode);
            break;
        case 'tip': document.getElementById("TipDiv").removeChild(div.parentNode.parentNode);
            break;
    }
}