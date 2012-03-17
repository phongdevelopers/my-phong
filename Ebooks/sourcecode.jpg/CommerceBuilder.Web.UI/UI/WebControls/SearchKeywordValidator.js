function SearchKeywordEvaluateIsValid(val)
{
    var keyword = document.getElementById(val.controltovalidate).value;
    if (keyword == "" || keyword.length == 0) return true;
    var cleanKeyword = keyword.replace(/[*\s]/g,"");
    var minimumLength = (+val.minimumLength);
    return (cleanKeyword.length >= minimumLength);
}

function SearchKeywordEvaluateIsValidRequired(val)
{
    var keyword = document.getElementById(val.controltovalidate).value;
    if (keyword == "" || keyword.length == 0) return false;
    var cleanKeyword = keyword.replace(/[*\s]/g,"");
    var minimumLength = (+val.minimumLength);
    return (cleanKeyword.length >= minimumLength);
}