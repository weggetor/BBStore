function CalculateTax(txtPercent,txtAmount,rdbNetto,rdbBrutto,hidBrutto, hidNetto) {
    var amount = 0;
    var Percent = 0;

    var sPercent = txtPercent.value;
    sPercent = sPercent.replace(",", ".");
    var sAmount = txtAmount.value;
    sAmount = sAmount.replace(",", ".");
    
    if (isNaN(sPercent) || (sPercent == "")) {
        Percent = 0;
    }
    else {
        Percent = parseFloat(sPercent);
    }
    if (isNaN(sAmount) || (sAmount == "")) {
        amount = 0;
    }
    else {
        amount = parseFloat(sAmount);
    }
    if (rdbBrutto.checked == true) {
        hidNetto.value = amount / (1 + Percent / 100);
        //        hidBrutto.value = Math.round(amount * 100) / 100;
        hidBrutto.value = amount.toFixed(2);
    }
    else {
        hidNetto.value = amount;
        // hidBrutto.value = Math.round(amount * (1 + Percent / 100) * 100) / 100;
        amount = amount * (1 + Percent / 100);
        hidBrutto.value = amount.toFixed(2);
    }
}
function ShowNet(txtPercent, txtAmount, rdbNetto, rdbBrutto, hidBrutto, hidNetto) {
    txtAmount.value = hidNetto.value;
}

function ShowGross(txtPercent, txtAmount, rdbNetto, rdbBrutto, hidBrutto, hidNetto) {
    txtAmount.value = hidBrutto.value;
}
