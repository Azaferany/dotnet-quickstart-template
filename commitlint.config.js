module.exports = {
    extends: ['@commitlint/config-conventional'], // My repo is using config-conventional you could use anything...
    rules: {
        'footer-max-line-length': [0, 'always'],
        'body-max-line-length': [0, 'always']
    },
};
