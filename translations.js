// ===== COMPLETE TRANSLATION SYSTEM - EVERY WORD! =====
const TRANSLATIONS = {
    en: {
        // ===== APP =====
        appTitle: "🍽️ Restaurant Management System",
        subtitle: "Manage your restaurant with ease and efficiency",
        launchDashboard: "🚀 Launch Dashboard",

        // ===== FEATURES =====
        featureUser: "User Management",
        featureUserDesc: "Manage staff with role-based access control",
        featureMenu: "Menu Control",
        featureMenuDesc: "Create and manage your restaurant menu",
        featureTable: "Table Tracking",
        featureTableDesc: "Monitor table availability in real-time",
        featureOrder: "Order Management",
        featureOrderDesc: "Track orders from kitchen to customer",

        // ===== AUTH =====
        loginTitle: "Login",
        registerTitle: "Register",
        username: "Username",
        password: "Password",
        email: "Email",
        fullName: "Full Name",
        role: "Role",
        loginBtn: "LOGIN",
        registerBtn: "REGISTER",
        noAccount: "Don't have an account?",
        haveAccount: "Already have an account?",
        registerHere: "Register here",
        loginHere: "Login here",
        forgotPassword: "Forgot Password?",
        forgotPasswordTitle: "Forgot Password",
        rememberPassword: "Remember your password?",
        sendOTP: "SEND OTP",
        resetPassword: "RESET PASSWORD",
        otpCode: "OTP Code",
        newPassword: "New Password",
        confirmPassword: "Confirm Password",
        currentPassword: "Current Password",

        // ===== NAVIGATION =====
        home: "🏠 Home",
        apiDocs: "📚 API Docs",
        settings: "⚙️ Settings",
        changePassword: "🔑 Change Password",
        editProfile: "👤 Edit Profile",
        logout: "🚪 Logout",

        // ===== TABS =====
        categories: "Categories",
        menuItems: "Menu Items",
        tables: "Tables",
        orders: "Orders",

        // ===== ACTIONS =====
        addCategory: "+ ADD CATEGORY",
        addMenuItem: "+ ADD MENU ITEM",
        addTable: "+ ADD TABLE",
        newOrder: "+ NEW ORDER",
        create: "Create",
        update: "Update",
        delete: "Delete",
        edit: "Edit",
        save: "Save Changes",
        cancel: "Cancel",
        toggle: "Toggle",
        close: "Close",
        confirm: "Confirm",
        back: "Back",

        // ===== CATEGORY LABELS =====
        categoryName: "Category Name",
        categoryNameAr: "Category Name (Arabic)",
        categoryDescription: "Category Description",
        displayOrder: "Display Order",
        itemsCount: "Items Count",

        // ===== MENU ITEM LABELS =====
        itemName: "Item Name",
        itemNameAr: "Item Name (Arabic)",
        itemDescription: "Description",
        price: "Price (EGP)",
        category: "Category",
        selectCategory: "Select Category",
        imageUrl: "Image URL",
        prepTime: "Preparation Time (minutes)",
        isAvailable: "Available",
        status: "Status",

        // ===== TABLE LABELS =====
        tableNumber: "Table Number",
        capacity: "Capacity",
        floorSection: "Floor Section",
        isOccupied: "Occupied",
        tableStatus: "Table Status",

        // ===== ORDER LABELS =====
        orderNumber: "Order Number",
        customerName: "Customer Name",
        selectTable: "Select Table",
        orderItems: "Order Items",
        addItem: "+ Add Item",
        selectItem: "Select Item",
        quantity: "Quantity",
        totalPrice: "Total Price",
        notes: "Notes",
        orderStatus: "Order Status",
        pending: "Pending",
        preparing: "Preparing",
        ready: "Ready",
        delivered: "Delivered",

        // ===== SETTINGS LABELS =====
        emailSettings: "Email Settings",
        smtpServer: "SMTP Server",
        smtpPort: "SMTP Port",
        fromEmail: "From Email",
        emailPassword: "Email Password",
        enableSSL: "Enable SSL",
        saveSettings: "SAVE SETTINGS",

        // ===== MODAL TITLES =====
        createCategory: "Create Category",
        editCategory: "Edit Category",
        createMenuItem: "Create Menu Item",
        editMenuItem: "Edit Menu Item",
        createTable: "Create Table",
        editTable: "Edit Table",
        createOrder: "Create Order",
        editOrder: "Edit Order",
        changePasswordTitle: "Change Password",
        editProfileTitle: "Edit Profile",
        deleteConfirmTitle: "Delete Confirmation",

        // ===== DELETE CONFIRMATION =====
        deleteConfirm: "Are you sure you want to delete",
        thisAction: "This action cannot be undone.",
        deleteYes: "Yes, Delete",
        deleteNo: "Cancel",

        // ===== MESSAGES =====
        loginSuccess: "Login successful!",
        loginFailed: "Login failed. Please check your credentials.",
        registerSuccess: "Registration successful!",
        registerFailed: "Registration failed. Please try again.",
        createSuccess: "Created successfully!",
        createFailed: "Failed to create. Please try again.",
        updateSuccess: "Updated successfully!",
        updateFailed: "Failed to update. Please try again.",
        deleteSuccess: "Deleted successfully!",
        deleteFailed: "Failed to delete. Please try again.",
        loadFailed: "Failed to load data. Please refresh the page.",
        loading: "Loading...",
        saving: "Saving...",
        deleting: "Deleting...",
        passwordChanged: "Password changed successfully!",
        profileUpdated: "Profile updated successfully!",
        otpSent: "OTP sent to your email!",
        otpVerified: "OTP verified successfully!",
        passwordReset: "Password reset successfully!",
        settingsSaved: "Settings saved successfully!",

        // ===== VALIDATION =====
        required: "This field is required",
        invalidEmail: "Please enter a valid email",
        passwordMismatch: "Passwords do not match",
        minLength: "Minimum length is",
        maxLength: "Maximum length is",

        // ===== STATUS =====
        available: "✓ AVAILABLE",
        unavailable: "✗ UNAVAILABLE",
        occupied: "OCCUPIED",
        vacant: "VACANT",
        active: "Active",
        inactive: "Inactive",

        // ===== MISC =====
        items: "Items",
        arabic: "Arabic",
        english: "English",
        admin: "Admin",
        user: "User",
        minutes: "mins",
        egp: "EGP",
        optional: "Optional",
        search: "Search...",
        noData: "No data available",
        page: "Page",
        of: "of",
        total: "Total",
        showing: "Showing",
        entries: "entries"
    },
    ar: {
        // ===== APP =====
        appTitle: "🍽️ نظام إدارة المطاعم",
        subtitle: "إدارة مطعمك بسهولة وكفاءة",
        launchDashboard: "🚀 تشغيل لوحة التحكم",

        // ===== FEATURES =====
        featureUser: "إدارة المستخدمين",
        featureUserDesc: "إدارة الموظفين بنظام صلاحيات محدد",
        featureMenu: "التحكم بالقائمة",
        featureMenuDesc: "إنشاء وإدارة قائمة المطعم",
        featureTable: "تتبع الطاولات",
        featureTableDesc: "مراقبة توفر الطاولات في الوقت الفعلي",
        featureOrder: "إدارة الطلبات",
        featureOrderDesc: "تتبع الطلبات من المطبخ إلى العميل",

        // ===== AUTH =====
        loginTitle: "تسجيل الدخول",
        registerTitle: "إنشاء حساب",
        username: "اسم المستخدم",
        password: "كلمة المرور",
        email: "البريد الإلكتروني",
        fullName: "الاسم الكامل",
        role: "الدور الوظيفي",
        loginBtn: "تسجيل الدخول",
        registerBtn: "إنشاء حساب",
        noAccount: "ليس لديك حساب؟",
        haveAccount: "لديك حساب بالفعل؟",
        registerHere: "سجل هنا",
        loginHere: "دخول هنا",
        forgotPassword: "هل نسيت كلمة المرور؟",
        forgotPasswordTitle: "استعادة كلمة المرور",
        rememberPassword: "هل تتذكر كلمة المرور؟",
        sendOTP: "إرسال رمز التحقق",
        resetPassword: "إعادة تعيين كلمة المرور",
        otpCode: "رمز التحقق",
        newPassword: "كلمة المرور الجديدة",
        confirmPassword: "تأكيد كلمة المرور",
        currentPassword: "كلمة المرور الحالية",

        // ===== NAVIGATION =====
        home: "🏠 الرئيسية",
        apiDocs: "📚 وثائق API",
        settings: "⚙️ الإعدادات",
        changePassword: "🔑 تغيير كلمة المرور",
        editProfile: "👤 تعديل الملف الشخصي",
        logout: "🚪 تسجيل الخروج",

        // ===== TABS =====
        categories: "الفئات",
        menuItems: "عناصر القائمة",
        tables: "الطاولات",
        orders: "الطلبات",

        // ===== ACTIONS =====
        addCategory: "+ إضافة فئة",
        addMenuItem: "+ إضافة عنصر",
        addTable: "+ إضافة طاولة",
        newOrder: "+ طلب جديد",
        create: "إنشاء",
        update: "تحديث",
        delete: "حذف",
        edit: "تعديل",
        save: "حفظ التغييرات",
        cancel: "إلغاء",
        toggle: "تبديل",
        close: "إغلاق",
        confirm: "تأكيد",
        back: "رجوع",

        // ===== CATEGORY LABELS =====
        categoryName: "اسم الفئة",
        categoryNameAr: "اسم الفئة (بالعربية)",
        categoryDescription: "وصف الفئة",
        displayOrder: "ترتيب العرض",
        itemsCount: "عدد العناصر",

        // ===== MENU ITEM LABELS =====
        itemName: "اسم العنصر",
        itemNameAr: "اسم العنصر (بالعربية)",
        itemDescription: "الوصف",
        price: "السعر (جنيه)",
        category: "الفئة",
        selectCategory: "اختر فئة",
        imageUrl: "رابط الصورة",
        prepTime: "وقت التحضير (دقائق)",
        isAvailable: "متاح",
        status: "الحالة",

        // ===== TABLE LABELS =====
        tableNumber: "رقم الطاولة",
        capacity: "السعة",
        floorSection: "قسم الطابق",
        isOccupied: "محجوزة",
        tableStatus: "حالة الطاولة",

        // ===== ORDER LABELS =====
        orderNumber: "رقم الطلب",
        customerName: "اسم العميل",
        selectTable: "اختر طاولة",
        orderItems: "عناصر الطلب",
        addItem: "+ إضافة عنصر",
        selectItem: "اختر عنصر",
        quantity: "الكمية",
        totalPrice: "السعر الإجمالي",
        notes: "ملاحظات",
        orderStatus: "حالة الطلب",
        pending: "قيد الانتظار",
        preparing: "قيد التحضير",
        ready: "جاهز",
        delivered: "تم التوصيل",

        // ===== SETTINGS LABELS =====
        emailSettings: "إعدادات البريد الإلكتروني",
        smtpServer: "خادم SMTP",
        smtpPort: "منفذ SMTP",
        fromEmail: "البريد المرسل",
        emailPassword: "كلمة مرور البريد",
        enableSSL: "تفعيل SSL",
        saveSettings: "حفظ الإعدادات",

        // ===== MODAL TITLES =====
        createCategory: "إنشاء فئة",
        editCategory: "تعديل الفئة",
        createMenuItem: "إنشاء عنصر قائمة",
        editMenuItem: "تعديل عنصر القائمة",
        createTable: "إنشاء طاولة",
        editTable: "تعديل الطاولة",
        createOrder: "إنشاء طلب",
        editOrder: "تعديل الطلب",
        changePasswordTitle: "تغيير كلمة المرور",
        editProfileTitle: "تعديل الملف الشخصي",
        deleteConfirmTitle: "تأكيد الحذف",

        // ===== DELETE CONFIRMATION =====
        deleteConfirm: "هل أنت متأكد من حذف",
        thisAction: "لا يمكن التراجع عن هذا الإجراء.",
        deleteYes: "نعم، احذف",
        deleteNo: "إلغاء",

        // ===== MESSAGES =====
        loginSuccess: "تم تسجيل الدخول بنجاح!",
        loginFailed: "فشل تسجيل الدخول. يرجى التحقق من بيانات الدخول.",
        registerSuccess: "تم إنشاء الحساب بنجاح!",
        registerFailed: "فشل إنشاء الحساب. يرجى المحاولة مرة أخرى.",
        createSuccess: "تم الإنشاء بنجاح!",
        createFailed: "فشل الإنشاء. يرجى المحاولة مرة أخرى.",
        updateSuccess: "تم التحديث بنجاح!",
        updateFailed: "فشل التحديث. يرجى المحاولة مرة أخرى.",
        deleteSuccess: "تم الحذف بنجاح!",
        deleteFailed: "فشل الحذف. يرجى المحاولة مرة أخرى.",
        loadFailed: "فشل تحميل البيانات. يرجى تحديث الصفحة.",
        loading: "جاري التحميل...",
        saving: "جاري الحفظ...",
        deleting: "جاري الحذف...",
        passwordChanged: "تم تغيير كلمة المرور بنجاح!",
        profileUpdated: "تم تحديث الملف الشخصي بنجاح!",
        otpSent: "تم إرسال رمز التحقق إلى بريدك الإلكتروني!",
        otpVerified: "تم التحقق من الرمز بنجاح!",
        passwordReset: "تم إعادة تعيين كلمة المرور بنجاح!",
        settingsSaved: "تم حفظ الإعدادات بنجاح!",

        // ===== VALIDATION =====
        required: "هذا الحقل مطلوب",
        invalidEmail: "يرجى إدخال بريد إلكتروني صحيح",
        passwordMismatch: "كلمات المرور غير متطابقة",
        minLength: "الحد الأدنى للطول هو",
        maxLength: "الحد الأقصى للطول هو",

        // ===== STATUS =====
        available: "✓ متاح",
        unavailable: "✗ غير متاح",
        occupied: "محجوزة",
        vacant: "شاغرة",
        active: "نشط",
        inactive: "غير نشط",

        // ===== MISC =====
        items: "عناصر",
        arabic: "عربي",
        english: "إنجليزي",
        admin: "مدير",
        user: "مستخدم",
        minutes: "دقيقة",
        egp: "جنيه",
        optional: "اختياري",
        search: "بحث...",
        noData: "لا توجد بيانات متاحة",
        page: "صفحة",
        of: "من",
        total: "المجموع",
        showing: "عرض",
        entries: "إدخالات"
    }
};

let currentLang = localStorage.getItem('language') || 'en';

// Cache in localStorage
if (!localStorage.getItem('translations_cache')) {
    localStorage.setItem('translations_cache', JSON.stringify(TRANSLATIONS));
}

// Get translation
function t(key) {
    const cached = JSON.parse(localStorage.getItem('translations_cache') || '{}');
    const lang = cached[currentLang] || TRANSLATIONS[currentLang];
    return lang[key] || key;
}

// Switch language
function switchLanguage(lang) {
    currentLang = lang;
    localStorage.setItem('language', lang);

    document.documentElement.setAttribute('dir', lang === 'ar' ? 'rtl' : 'ltr');
    document.documentElement.setAttribute('lang', lang);

    document.querySelectorAll('.lang-btn').forEach(btn => btn.classList.remove('active'));
    const activeBtn = document.getElementById('lang' + lang.toUpperCase());
    if (activeBtn) activeBtn.classList.add('active');

    updateAllTranslations();
}

// Update translations
function updateAllTranslations() {
    const cached = JSON.parse(localStorage.getItem('translations_cache'));
    const translations = cached[currentLang] || TRANSLATIONS[currentLang];

    // Update text content
    document.querySelectorAll('[data-i18n]').forEach(element => {
        const key = element.getAttribute('data-i18n');
        if (translations[key]) {
            element.textContent = translations[key];
        }
    });

    // Update placeholders
    document.querySelectorAll('[data-i18n-placeholder]').forEach(element => {
        const key = element.getAttribute('data-i18n-placeholder');
        if (translations[key]) {
            element.placeholder = translations[key];
        }
    });
}

// Initialize
function initializeTranslations() {
    const savedLang = localStorage.getItem('language') || 'en';
    switchLanguage(savedLang);
}

// Auto-initialize
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeTranslations);
} else {
    initializeTranslations();
}