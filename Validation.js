// ===== REAL-TIME FORM VALIDATION MODULE =====
// Add this to your app.js or create a separate validation.js file

const FormValidator = {
    // Email validation regex
    emailRegex: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,

    // Password requirements
    passwordRequirements: {
        minLength: 8,
        hasUpperCase: /[A-Z]/,
        hasLowerCase: /[a-z]/,
        hasNumber: /[0-9]/,
        hasSpecial: /[!@#$%^&*(),.?":{}|<>]/
    },

    // Initialize validation for a form
    init(formId) {
        const form = document.getElementById(formId);
        if (!form) return;

        // Add validation icons and message containers to all inputs
        form.querySelectorAll('.form-group').forEach(group => {
            const input = group.querySelector('input, select, textarea');
            if (!input) return;

            // Add has-label class if label exists
            if (group.querySelector('label')) {
                group.classList.add('has-label');
            }

            // Add validation icon
            if (!group.querySelector('.validation-icon')) {
                const icon = document.createElement('span');
                icon.className = 'validation-icon';
                group.appendChild(icon);
            }

            // Add validation message container
            if (!group.querySelector('.validation-message')) {
                const message = document.createElement('span');
                message.className = 'validation-message';
                group.appendChild(message);
            }

            // Attach event listeners
            input.addEventListener('input', () => this.validateField(input));
            input.addEventListener('blur', () => this.validateField(input, true));
            input.addEventListener('focus', () => this.clearFieldError(input));
        });

        console.log('✅ Form validation initialized for:', formId);
    },

    // Validate a single field
    validateField(input, showError = false) {
        const formGroup = input.closest('.form-group');
        const value = input.value.trim();
        const inputType = input.type;
        const inputId = input.id;

        // Clear previous states
        formGroup.classList.remove('valid', 'invalid', 'match', 'mismatch');

        // Skip validation if empty and not required
        if (!value && !input.required) {
            this.clearFieldError(input);
            return true;
        }

        let isValid = true;
        let message = '';

        // Validate based on input type/id
        switch (inputType) {
            case 'email':
                isValid = this.emailRegex.test(value);
                message = isValid ?
                    '✓ Valid email address' :
                    '✗ Please enter a valid email address';
                this.showEmailStatus(formGroup, isValid);
                break;

            case 'password':
                if (inputId.includes('confirm') || inputId.includes('Confirm')) {
                    // Password confirmation
                    const passwordField = this.getPasswordField(input);
                    if (passwordField) {
                        isValid = value === passwordField.value;
                        message = isValid ?
                            '✓ Passwords match' :
                            '✗ Passwords do not match';
                        formGroup.classList.add(isValid ? 'match' : 'mismatch');
                    }
                } else {
                    // Regular password validation
                    const strength = this.checkPasswordStrength(value);
                    isValid = strength.score >= 2; // At least medium strength
                    message = strength.message;
                    this.showPasswordStrength(formGroup, strength);
                }
                break;

            case 'text':
                // Username validation
                if (inputId.includes('username') || inputId.includes('Username')) {
                    isValid = value.length >= 3 && /^[a-zA-Z0-9_]+$/.test(value);
                    message = isValid ?
                        '✓ Valid username' :
                        '✗ Username must be 3+ characters (letters, numbers, underscore only)';

                    // Async check for availability
                    if (isValid && showError) {
                        this.checkUsernameAvailability(input, value);
                    }
                }
                // Full name validation
                else if (inputId.includes('fullName') || inputId.includes('FullName')) {
                    isValid = value.length >= 3 && /^[a-zA-Z\s]+$/.test(value);
                    message = isValid ?
                        '✓ Valid name' :
                        '✗ Name must be 3+ characters (letters and spaces only)';
                }
                break;

            case 'number':
                if (value) {
                    const num = parseFloat(value);
                    const min = parseFloat(input.min);
                    const max = parseFloat(input.max);

                    if (min !== undefined && num < min) {
                        isValid = false;
                        message = `✗ Minimum value is ${min}`;
                    } else if (max !== undefined && num > max) {
                        isValid = false;
                        message = `✗ Maximum value is ${max}`;
                    } else {
                        isValid = true;
                        message = '✓ Valid number';
                    }
                }
                break;
        }

        // Update UI
        if (value) {
            formGroup.classList.add(isValid ? 'valid' : 'invalid');

            if (showError || isValid) {
                const messageEl = formGroup.querySelector('.validation-message');
                if (messageEl) {
                    messageEl.textContent = message;
                    messageEl.className = `validation-message ${isValid ? 'success-message' : 'error-message'}`;
                }
            }
        }

        return isValid;
    },

    // Check password strength
    checkPasswordStrength(password) {
        const reqs = this.passwordRequirements;
        let score = 0;
        let feedback = [];

        // Check length
        if (password.length >= reqs.minLength) {
            score++;
        } else {
            feedback.push(`At least ${reqs.minLength} characters`);
        }

        // Check uppercase
        if (reqs.hasUpperCase.test(password)) {
            score++;
        } else {
            feedback.push('One uppercase letter');
        }

        // Check lowercase
        if (reqs.hasLowerCase.test(password)) {
            score++;
        } else {
            feedback.push('One lowercase letter');
        }

        // Check number
        if (reqs.hasNumber.test(password)) {
            score++;
        } else {
            feedback.push('One number');
        }

        // Check special character
        if (reqs.hasSpecial.test(password)) {
            score++;
        } else {
            feedback.push('One special character');
        }

        // Determine strength
        let strength, message;
        if (score <= 2) {
            strength = 'weak';
            message = '✗ Weak password';
        } else if (score <= 3) {
            strength = 'medium';
            message = '⚠ Medium strength';
        } else {
            strength = 'strong';
            message = '✓ Strong password';
        }

        return { score, strength, message, feedback };
    },

    // Show password strength indicator
    showPasswordStrength(formGroup, strengthData) {
        let strengthContainer = formGroup.querySelector('.password-strength');

        if (!strengthContainer) {
            strengthContainer = document.createElement('div');
            strengthContainer.className = 'password-strength';
            strengthContainer.innerHTML = `
                <div class="password-strength-bar"></div>
            `;

            const strengthText = document.createElement('span');
            strengthText.className = 'password-strength-text';

            formGroup.appendChild(strengthContainer);
            formGroup.appendChild(strengthText);

            // Add requirements checklist
            const requirements = document.createElement('div');
            requirements.className = 'password-requirements';
            requirements.innerHTML = `
                <h4>Password Requirements:</h4>
                <div class="requirement-item" data-req="length">
                    <span class="icon"></span>
                    <span>At least ${this.passwordRequirements.minLength} characters</span>
                </div>
                <div class="requirement-item" data-req="uppercase">
                    <span class="icon"></span>
                    <span>One uppercase letter</span>
                </div>
                <div class="requirement-item" data-req="lowercase">
                    <span class="icon"></span>
                    <span>One lowercase letter</span>
                </div>
                <div class="requirement-item" data-req="number">
                    <span class="icon"></span>
                    <span>One number</span>
                </div>
                <div class="requirement-item" data-req="special">
                    <span class="icon"></span>
                    <span>One special character (!@#$%...)</span>
                </div>
            `;
            formGroup.appendChild(requirements);
        }

        // Update strength bar
        const bar = strengthContainer.querySelector('.password-strength-bar');
        const text = formGroup.querySelector('.password-strength-text');

        bar.className = `password-strength-bar ${strengthData.strength}`;
        text.className = `password-strength-text ${strengthData.strength}`;
        text.textContent = strengthData.message;

        // Update requirements checklist
        const input = formGroup.querySelector('input');
        const password = input.value;

        const reqItems = formGroup.querySelectorAll('.requirement-item');
        reqItems.forEach(item => {
            const req = item.dataset.req;
            let isMet = false;

            switch (req) {
                case 'length':
                    isMet = password.length >= this.passwordRequirements.minLength;
                    break;
                case 'uppercase':
                    isMet = this.passwordRequirements.hasUpperCase.test(password);
                    break;
                case 'lowercase':
                    isMet = this.passwordRequirements.hasLowerCase.test(password);
                    break;
                case 'number':
                    isMet = this.passwordRequirements.hasNumber.test(password);
                    break;
                case 'special':
                    isMet = this.passwordRequirements.hasSpecial.test(password);
                    break;
            }

            item.classList.toggle('met', isMet);
        });
    },

    // Show email validation status
    showEmailStatus(formGroup, isValid) {
        let statusEl = formGroup.querySelector('.email-status');

        if (!statusEl) {
            statusEl = document.createElement('div');
            statusEl.className = 'email-status';
            formGroup.appendChild(statusEl);
        }

        statusEl.className = `email-status ${isValid ? 'valid' : 'invalid'}`;
        statusEl.textContent = isValid ? '✓ Valid format' : '✗ Invalid format';
    },

    // Check username availability (simulated - replace with actual API call)
    async checkUsernameAvailability(input, username) {
        const formGroup = input.closest('.form-group');

        // Add async validation indicator
        let asyncEl = formGroup.querySelector('.async-validation');
        if (!asyncEl) {
            asyncEl = document.createElement('div');
            asyncEl.className = 'async-validation';
            formGroup.appendChild(asyncEl);
        }

        // Show checking state
        asyncEl.className = 'async-validation checking';
        asyncEl.innerHTML = '<span class="spinner"></span> Checking availability...';

        // Simulate API call (replace with actual API)
        setTimeout(() => {
            // For demo: usernames starting with 'admin' are taken
            const isTaken = username.toLowerCase().startsWith('admin');

            asyncEl.className = `async-validation ${isTaken ? 'taken' : 'available'}`;
            asyncEl.textContent = isTaken ?
                '✗ Username is taken' :
                '✓ Username is available';

            // Update form group state
            formGroup.classList.remove('valid', 'invalid');
            formGroup.classList.add(isTaken ? 'invalid' : 'valid');
        }, 1000);
    },

    // Get corresponding password field for confirmation
    getPasswordField(confirmInput) {
        const formGroup = confirmInput.closest('form');
        if (!formGroup) return null;

        // Try to find password field
        const passwordFields = formGroup.querySelectorAll('input[type="password"]');
        for (let field of passwordFields) {
            if (!field.id.includes('confirm') && !field.id.includes('Confirm')) {
                return field;
            }
        }
        return null;
    },

    // Clear field error state
    clearFieldError(input) {
        const formGroup = input.closest('.form-group');
        formGroup.classList.remove('invalid');

        const message = formGroup.querySelector('.validation-message');
        if (message && message.classList.contains('error-message')) {
            message.textContent = '';
            message.className = 'validation-message';
        }
    },

    // Validate entire form
    validateForm(formId) {
        const form = document.getElementById(formId);
        if (!form) return false;

        let isValid = true;
        const inputs = form.querySelectorAll('input[required], select[required], textarea[required]');

        inputs.forEach(input => {
            const fieldValid = this.validateField(input, true);
            if (!fieldValid) {
                isValid = false;
            }
        });

        return isValid;
    },

    // Show form-level success message
    showFormSuccess(form, message = 'Success!') {
        const existingMsg = form.querySelector('.success-message-box');
        if (existingMsg) existingMsg.remove();

        const successBox = document.createElement('div');
        successBox.className = 'success-message-box';
        successBox.innerHTML = `
            <div class="icon">✓</div>
            <div class="message">${message}</div>
        `;

        form.insertBefore(successBox, form.firstChild);

        // Auto-remove after 5 seconds
        setTimeout(() => {
            successBox.style.opacity = '0';
            setTimeout(() => successBox.remove(), 300);
        }, 5000);
    },

    // Show form-level error message
    showFormError(form, message = 'Please fix the errors below') {
        const existingMsg = form.querySelector('.error-message-box');
        if (existingMsg) existingMsg.remove();

        const errorBox = document.createElement('div');
        errorBox.className = 'error-message-box';
        errorBox.innerHTML = `
            <div class="icon">✗</div>
            <div class="message">${message}</div>
        `;

        form.insertBefore(errorBox, form.firstChild);

        // Auto-remove after 5 seconds
        setTimeout(() => {
            errorBox.style.opacity = '0';
            setTimeout(() => errorBox.remove(), 300);
        }, 5000);
    },

    // Add password visibility toggle
    addPasswordToggle(passwordInputId) {
        const input = document.getElementById(passwordInputId);
        if (!input) return;

        const formGroup = input.closest('.form-group');

        // Check if toggle already exists
        if (formGroup.querySelector('.toggle-password')) return;

        const toggle = document.createElement('button');
        toggle.type = 'button';
        toggle.className = 'toggle-password';
        toggle.innerHTML = '👁';
        toggle.setAttribute('title', 'Show/hide password');

        toggle.addEventListener('click', () => {
            if (input.type === 'password') {
                input.type = 'text';
                toggle.innerHTML = '👁‍🗨';
            } else {
                input.type = 'password';
                toggle.innerHTML = '👁';
            }
        });

        formGroup.appendChild(toggle);
    }
};

// Auto-initialize validation when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    // Initialize for all forms
    ['loginForm', 'registerForm', 'forgotPasswordForm', 'resetPasswordForm',
        'changePasswordForm', 'createCategoryForm', 'createMenuItemForm',
        'createTableForm', 'createOrderForm', 'emailSettingsForm'].forEach(formId => {
            FormValidator.init(formId);
        });

    // Add password toggles
    ['loginPassword', 'regPassword', 'newPassword', 'confirmPasswordReset',
        'newPasswordChange', 'confirmPasswordChange', 'currentPassword'].forEach(id => {
            FormValidator.addPasswordToggle(id);
        });

    console.log('✅ Real-time validation system initialized');
});

// Export for use in other scripts
window.FormValidator = FormValidator;