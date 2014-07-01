function GetIban (lkz, txtBin, txtKto, txtIban) {
    var bban = txtBin.value + (txtKto.value + "0000000").substr(0, 10);
    var clkz = lkz.trim().toUpperCase().split('');
    var nlkz = (clkz[0].charCodeAt(0) - 'A'.charCodeAt(0) + 10).toString() + (clkz[1].charCodeAt(0) - 'A'.charCodeAt(0) + 10).toString() + "00";
    var checksum = bban + nlkz;
    var nPruef = 98 - mod97(checksum);
    if (nPruef <= 9 )
        txtIban.value = lkz.toUpperCase() + "0" + nPruef.toString() + bban;
    else
        txtIban.value = lkz.toUpperCase()  + nPruef.toString() + bban;
}

// Modulo 97 for huge numbers given as digit strings.
function mod97(digit_string) {
    var m = 0;
    for (var i = 0; i < digit_string.length; ++i)
        m = (m * 10 + parseInt(digit_string.charAt(i))) % 97;
    return m;
}