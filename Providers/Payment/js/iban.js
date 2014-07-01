// iban.html & iban.js 1.5 - Create or check International Bank Account Numbers
// Copyright (C) 2002-2010, Thomas Günther <tom@toms-cafe.de>

// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.


// Interface functions for iban.html:
// CreateIBAN
// CheckIBAN
// WriteCountrySelectionBar
// WriteCountryFormatTable
// WriteExampleTestTable

// Required tags in iban.html:
// Form             ibanform
//   Selection bar  ibanform.country
//   Input field    ibanform.bank
//   Input field    ibanform.account
//   Input field    ibanform.iban
//   Output field   ibanform.alt_iban
// Image            bban_img
// Image            iban_img

// Used images:
// okay.png         check-mark in ibanform
// error.png        question-mark in ibanform
// blank.png        erase check-mark and question-mark
// arrows_lr.png    arrows in examples table



// JavaScript Object for country specific iban data.
function Country(name, code, bankForm, accForm) {
    // Constructor for Country objects.
    //
    // Arguments:
    //   name      - Name of the country
    //   code      - Country Code from ISO 3166
    //   bank_form - Format of bank/branch code part (e.g. "0 4a 0 ")
    //   acc_form  - Format of account number part (e.g. "0  11  2n")

    this.name = name;
    this.code = code;
    this.bank = Country_decode_format(bankForm);
    this.acc = Country_decode_format(accForm);
    this.bank_lng = Country_calc_length(this.bank);
    this.acc_lng = Country_calc_length(this.acc);
    this.total_lng = 4 + this.bank_lng + this.acc_lng;
}

function Country_decode_format(form) {
    var formList = new Array();
    var parts = form.split(" ");
    for (var i = 0; i < parts.length; ++i) {
        var part = parts[i];
        if (part != "") {
            var typ = part.charAt(part.length - 1);
            if (typ == "a" || typ == "n")
                part = part.substring(0, part.length - 1);
            else
                typ = "c";
            var lng = parseInt(part);
            formList[formList.length] = new Array(lng, typ);
        }
    }
    return formList;
}

function Country_calc_length(formList) {
    var sum = 0;
    for (var i = 0; i < formList.length; ++i)
        sum += formList[i][0];
    return sum;
}

// BBAN data from ISO 13616, Country codes from ISO 3166 (www.iso.org).
var iban_data = new Array(
  new Country("Andorra", "AD", "0  4n 4n", "0  12   0 "),
  new Country("Albania", "AL", "0  8n 0 ", "0  16   0 "),
  new Country("Austria", "AT", "0  5n 0 ", "0  11n  0 "),
  new Country("Bosnia and Herzegovina",
                                "BA", "0  3n 3n", "0   8n  2n"),
  new Country("Belgium", "BE", "0  3n 0 ", "0   7n  2n"),
  new Country("Bulgaria", "BG", "0  4a 4n", "2n  8   0 "),
  new Country("Switzerland", "CH", "0  5n 0 ", "0  12   0 "),
  new Country("Cyprus", "CY", "0  3n 5n", "0  16   0 "),
  new Country("Czech Republic", "CZ", "0  4n 0 ", "0  16n  0 "),
  new Country("Germany", "DE", "0  8n 0 ", "0  10n  0 "),
  new Country("Denmark", "DK", "0  4n 0 ", "0   9n  1n"),
  new Country("Estonia", "EE", "0  2n 0 ", "2n 11n  1n"),
  new Country("Spain", "ES", "0  4n 4n", "2n 10n  0 "),
  new Country("Finland", "FI", "0  6n 0 ", "0   7n  1n"),
  new Country("Faroe Islands", "FO", "0  4n 0 ", "0   9n  1n"),
  new Country("France", "FR", "0  5n 5n", "0  11   2n"),
  new Country("United Kingdom", "GB", "0  4a 6n", "0   8n  0 "),
  new Country("Georgia", "GE", "0  2a 0 ", "0  16n  0 "),
  new Country("Gibraltar", "GI", "0  4a 0 ", "0  15   0 "),
  new Country("Greenland", "GL", "0  4n 0 ", "0   9n  1n"),
  new Country("Greece", "GR", "0  3n 4n", "0  16   0 "),
  new Country("Croatia", "HR", "0  7n 0 ", "0  10n  0 "),
  new Country("Hungary", "HU", "0  3n 4n", "1n 15n  1n"),
  new Country("Ireland", "IE", "0  4a 6n", "0   8n  0 "),
  new Country("Israel", "IL", "0  3n 3n", "0  13n  0 "),
  new Country("Iceland", "IS", "0  4n 0 ", "2n 16n  0 "),
  new Country("Italy", "IT", "1a 5n 5n", "0  12   0 "),
  new Country("Kuwait", "KW", "0  4a 0 ", "0  22   0 "),
  new Country("Kazakhstan", "KZ", "0  3n 0 ", "0  13   0 "),
  new Country("Lebanon", "LB", "0  4n 0 ", "0  20   0 "),
  new Country("Liechtenstein", "LI", "0  5n 0 ", "0  12   0 "),
  new Country("Lithuania", "LT", "0  5n 0 ", "0  11n  0 "),
  new Country("Luxembourg", "LU", "0  3n 0 ", "0  13   0 "),
  new Country("Latvia", "LV", "0  4a 0 ", "0  13   0 "),
  new Country("Monaco", "MC", "0  5n 5n", "0  11   2n"),
  new Country("Montenegro", "ME", "0  3n 0 ", "0  13n  2n"),
  new Country("Macedonia, Former Yugoslav Republic of",
                                "MK", "0  3n 0 ", "0  10   2n"),
  new Country("Mauritania", "MR", "0  5n 5n", "0  11n  2n"),
  new Country("Malta", "MT", "0  4a 5n", "0  18   0 "),
  new Country("Mauritius", "MU", "0  4a 4n", "0  15n  3a"),
  new Country("Netherlands", "NL", "0  4a 0 ", "0  10n  0 "),
  new Country("Norway", "NO", "0  4n 0 ", "0   6n  1n"),
  new Country("Poland", "PL", "0  8n 0 ", "0  16n  0 "),
  new Country("Portugal", "PT", "0  4n 4n", "0  11n  2n"),
  new Country("Romania", "RO", "0  4a 0 ", "0  16   0 "),
  new Country("Serbia", "RS", "0  3n 0 ", "0  13n  2n"),
  new Country("Saudi Arabia", "SA", "0  2n 0 ", "0  18   0 "),
  new Country("Sweden", "SE", "0  3n 0 ", "0  16n  1n"),
  new Country("Slovenia", "SI", "0  5n 0 ", "0   8n  2n"),
  new Country("Slovak Republic",
                                "SK", "0  4n 0 ", "0  16n  0 "),
  new Country("San Marino", "SM", "1a 5n 5n", "0  12   0 "),
  new Country("Tunisia", "TN", "0  2n 3n", "0  13n  2n"),
  new Country("Turkey", "TR", "0  5n 0 ", "1  16   0 "));

// Search the country code in the iban_data list.
function CountryData(code) {
    for (var i = 0; i < iban_data.length; ++i)
        if (iban_data[i].code == code)
            return iban_data[i];
    return null;
}

// Modulo 97 for huge numbers given as digit strings.
function mod97(digitString) {
    var m = 0;
    for (var i = 0; i < digitString.length; ++i)
        m = (m * 10 + parseInt(digitString.charAt(i))) % 97;
    return m;
}

// Convert a capital letter into digits: A -> 10 ... Z -> 35 (ISO 13616).
function capital2digits(ch) {
    var capitals = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    for (var i = 0; i < capitals.length; ++i)
        if (ch == capitals.charAt(i))
            break;
    return i + 10;
}

// Fill the string with leading zeros until length is reached.
function fill0(s, l) {
    while (s.length < l)
        s = "0" + s;
    return s;
}

// Compare two strings respecting german umlauts.
function strcmp(s1, s2) {
    var chars = "AaÄäBbCcDdEeFfGgHhIiJjKkLlMmNnOoÖöPpQqRrSsßTtUuÜüVvWwXxYyZz";
    var lng = (s1.length < s2.length) ? s1.length : s2.length;
    for (var i = 0; i < lng; ++i) {
        var d = chars.indexOf(s1.charAt(i)) - chars.indexOf(s2.charAt(i));
        if (d != 0)
            return d;
    }
    return s1.length - s2.length;
}

// Create an index table of the iban_data list sorted by country names.
function CountryIndexTable() {
    var tab = new Array();
    var i, j, t;
    for (i = 0; i < iban_data.length; ++i)
        tab[i] = i;
    for (i = tab.length - 1; i > 0; --i)
        for (j = 0; j < i; ++j)
            if (strcmp(iban_data[tab[j]].name, iban_data[tab[j + 1]].name) > 0)
                t = tab[j], tab[j] = tab[j + 1], tab[j + 1] = t;
    return tab;
}

// Calculate 2-digit checksum of an IBAN.
function ChecksumIBAN(iban) {
    var code = iban.substring(0, 2);
    var checksum = iban.substring(2, 4);
    var bban = iban.substring(4);

    // Assemble digit string
    var digits = "";
    for (var i = 0; i < bban.length; ++i) {
        var ch = bban.charAt(i).toUpperCase();
        if ("0" <= ch && ch <= "9")
            digits += ch;
        else
            digits += capital2digits(ch);
    }
    for (var i = 0; i < code.length; ++i) {
        var ch = code.charAt(i);
        digits += capital2digits(ch);
    }
    digits += checksum;

    // Calculate checksum
    checksum = 98 - mod97(digits);
    return fill0("" + checksum, 2);
}

// Fill the account number part of IBAN with leading zeros.
function FillAccount(country, account) {
    return fill0(account, country.acc_lng);
}

// Check if syntax of the part of IBAN is invalid.
function InvalidPart(form_list, iban_part) {
    for (var f = 0; f < form_list.length; ++f) {
        var lng = form_list[f][0], typ = form_list[f][1];
        if (lng > iban_part.length)
            lng = iban_part.length;
        for (var i = 0; i < lng; ++i) {
            var ch = iban_part.charAt(i);
            var a = ("A" <= ch && ch <= "Z");
            var n = ("0" <= ch && ch <= "9");
            var c = n || a || ("a" <= ch && ch <= "z");
            if ((!c && typ == "c") || (!a && typ == "a") || (!n && typ == "n"))
                return true;
        }
        iban_part = iban_part.substring(lng);
    }
    return false;
}

// Check if length of the bank/branch code part of IBAN is invalid.
function InvalidBankLength(country, bank) {
    return (bank.length != country.bank_lng);
}

// Check if syntax of the bank/branch code part of IBAN is invalid.
function InvalidBank(country, bank) {
    return (InvalidBankLength(country, bank) ||
            InvalidPart(country.bank, bank));
}

// Check if length of the account number part of IBAN is invalid.
function InvalidAccountLength(country, account) {
    return (account.length < 1 || account.length > country.acc_lng);
}

// Check if syntax of the account number part of IBAN is invalid.
function InvalidAccount(country, account) {
    return (InvalidAccountLength(country, account) ||
            InvalidPart(country.acc, FillAccount(country, account)));
}

// Check if length of IBAN is invalid.
function InvalidIBANlength(country, iban) {
    return (iban.length != country.total_lng);
}

// Convert iban from intern value to string format (IBAN XXXX XXXX ...).
function extern(intern) {
    var s = "";
    for (var i = 0; i < intern.length; ++i) {
        if (i % 4 == 3)
            s += " ";
        s += intern.charAt(i);
    }
    return s;
}

// Convert iban from string format to intern value.
function intern(extern) {
    if (extern.substring(0, 4) == "IBAN")
        extern = extern.substring(4);
    var s = "";
    for (var i = 0; i < extern.length; ++i)
        if (extern.charAt(i) != " ")
            s += extern.charAt(i);
    return s;
}

// Calculate the checksum and assemble the IBAN.
function CalcIBAN(country, bank, account) {
    var fillAcc = FillAccount(country, account);
    var checksum = ChecksumIBAN(country.code + "00" + bank + fillAcc);
    return country.code + checksum + bank + fillAcc;
}

function CalcAltIBAN(country, bank, account) {
    var fillAcc = FillAccount(country, account);
    var checksum = ChecksumIBAN(country.code + "00" + bank + fillAcc);
    checksum = fill0("" + mod97(checksum), 2);
    return country.code + checksum + bank + fillAcc;
}

// Check the checksum of an IBAN.
function IBANokay(iban) {
    return ChecksumIBAN(iban) == "97";
}

// Check the input, calculate the checksum and assemble the IBAN.
function CreateIBAN(ddlCountry,txtBin,txtAccountNo,txtIban, hidIbanOk) {

    var code = ddlCountry.options[ddlCountry.selectedIndex].value;
    var bank = intern(txtBin.value);
    var account = intern(txtAccountNo.value);
    var country = CountryData(code);

    var err = null, errFocus = null;
    if (country == null) {
        err = _("Unknown Country Code: ") + code;
        errFocus = ddlCountry;
    }
    else if (InvalidBankLength(country, bank)) {
        err = _("Bank/Branch Code length ") + bank.length +
              _(" is not correct for ") + country.name +
              " (" + country.bank_lng + ")";
        errFocus = txtBin;
    }
    else if (InvalidBank(country, bank)) {
        err = _("Bank/Branch Code ") + bank + _(" is not correct for ") +
              country.name;
        errFocus = txtBin;
    }
    else if (InvalidAccountLength(country, account)) {
        err = _("Account Number length ") + account.length +
              _(" is not correct for ") + country.name +
              " (" + country.acc_lng + ")";
        errFocus = txtAccountNo;
    }
    else if (InvalidAccount(country, account)) {
        err = _("Account Number ") + account + _(" is not correct for ") +
              country.name;
        errFocus = txtAccountNo;
    }

    if (err) {
 
        // Clear destination fields, set focus to wrong field
        txtIban.value = "";
        errFocus.focus();
        hidIbanOk.value = "false";

        // Show message box with error message
        alert(err);
    }
    else {

        // Calculate IBAN, write results in form fields
        txtBin.value = bank;
        txtAccountNo.value = FillAccount(country, account);
        txtIban.value = extern(CalcIBAN(country, bank, account));
        hidIbanOK.value = "true";

        // Check for dispensable global variables in debug modus
        if (debug_output)
            debug_check_vars();
    }
}
// Check BIC
function CheckBic(txtBic) {
    var bic = txtBic.value;
    if (bic.length != 8 && bic.length != 11) {
        // Show message box with error message
        alert("BIC length must be 8 or 11!");
        txtBic.focus();
    }
}


// Check the syntax and the checksum of the IBAN.
function CheckIBAN(ddlCountry, txtBin, txtAccountNo, txtIban, hidIbanOk) {
    
    var iban = intern(txtIban.value);

    var code = iban.substring(0, 2);
    var checksum = iban.substring(2, 4);
    var bban = iban.substring(4);
    var country = CountryData(code);

    var err = null;
    if (country == null)
        err = _("Unknown Country Code: ") + code;
    else if (InvalidIBANlength(country, iban))
        err = _("IBAN length ") + iban.length + _(" is not correct for ") +
              country.name + " (" + country.total_lng + ")";
    else {
        var bankLng = country.bank_lng;
        var bank = bban.substring(0, bankLng);
        var account = bban.substring(bankLng);

        if (InvalidBank(country, bank))
            err = _("Bank/Branch Code ") + bank + _(" is not correct for ") +
                  country.name;
        else if (InvalidAccount(country, account))
            err = _("Account Number ") + account + _(" is not correct for ") +
                  country.name;
        else if (!IBANokay(iban))
            err = _("Checksum of IBAN incorrect");
    }

    if (err) {

        // Clear destination fields, set focus to wrong field
        ddlCountry.selectedIndex = 0;
        txtBin.value = "";
        txtAccountNo.value = "";
        hidIbanOk.value = "false";

        // Show message box with error message
        alert(err);
    }
    else {

        // Write results in form fields
        txtIban.value = extern(iban);
        for (var i = ddlCountry.options.length - 1; i > 0; --i)
            if (ddlCountry.options[i].value == code)
                break;
        ddlCountry.selectedIndex = i;
        txtBin.value = bank;
        txtAccountNo.value = account;
        hidIbanOk.value = "true";

        // Check for dispensable global variables in debug modus
        if (debug_output)
            debug_check_vars();
    }
}

// Write the selection bar into the form.
function WriteCountrySelectionBar() {
    document.write('<select name="country" size="1">');
    document.write('<option value="??">&nbsp;</option>');
    var tab = CountryIndexTable();
    for (var i = 0; i < tab.length; ++i) {
        var country = iban_data[tab[i]];
        document.write('<option value="' + country.code + '">' +
                       country.name + ' (' + country.code + ')</option>');
    }
    document.write('</select>');
}


// Translation table and translation function for localized versions
var trans_tab = new Array();

function _(s) {
    var t = trans_tab[s];
    if (t)
        s = t;
    return s;
}

// Fill the translation table
function fill_trans_tab(trans_data) {
    for (var i = 0; i < trans_data.length / 2; ++i)
        trans_tab[trans_data[2 * i]] = trans_data[2 * i + 1];

    // Translate the country names in the iban_data list
    for (var i = 0; i < iban_data.length; ++i)
        iban_data[i].name = _(iban_data[i].name);
}


// Set debug_output = true if location ends with a hash or a quotation mark
var debug_output = (location.href.charAt(location.href.length - 1) == "#") ||
                   (location.href.charAt(location.href.length - 1) == "?");

if (debug_output)
    debug_iban_data();

function debug_iban_data() {
    var s = "";
    for (var i = 0; i < iban_data.length; ++i) {
        var country = iban_data[i];
        s += country.name + " / " + country.code + " / ";
        for (var f = 0; f < country.bank.length; ++f)
            s += country.bank[f][0] + country.bank[f][1];
        s += " = " + country.bank_lng + " / ";
        for (var f = 0; f < country.acc.length; ++f)
            s += country.acc[f][0] + country.acc[f][1];
        s += " = " + country.acc_lng + " / " + country.total_lng + "\n";
    }
    alert(s);
}

function debug_check_vars() {
    var o = false;
    var s = "";
    for (var v in window) {
        if (o)
            s += "" + v + "=" + window[v] + "\n";
        if (v == "debug_check_vars")
            o = true;
    }
    if (s != "")
        alert("vars:\n" + s);
    else
        alert("no vars");
}
