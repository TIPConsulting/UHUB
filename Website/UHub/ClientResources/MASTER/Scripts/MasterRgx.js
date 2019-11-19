var RgxPtrns = {

    User: {
        EMAIL: /^[\S]+@([\w-]+\.)+[\w-]+$/,
        USERNAME: /^\S{3,50}$/,
        PSWD: /^.{8,150}$/,
        NAME: /^(([ \u00c0-\u01ffA-z'\-])+){2,200}$/,
        MAJOR: /^.{2,250}$/,
        PHONE: /^([0-1][ .-])?((\([0-9]{3}\)[ .-]?)|([0-9]{3}[ .-]?))([0-9]{3}[ .-]?)([0-9]{4})$/,
        GRAD_DATE: /^[0-9]{4}\-[0-1][0-9]\-[0-3][0-9]$/,
        YEAR: /^Freshman|Sophomore|Junior|Senior\+?$/,
        COMPANY: /^.{0,100}$/,
        JOB_TITLE: /^.{0,100}$/
    },
    Club: {
        NAME: /^(([ \u00c0-\u01ffA-z0-9'\-])+){3,100}$/,
        DESCRIPTION: /^[\s\S]{0,2000}$/
    },
    Post: {
        NAME: /^.{1,100}$/,
        CONTENT: /^[\s\S]{10,10000}$/
    }
};