function getAuthHeaders() {
    // Refresh token from localStorage every time
    authToken = localStorage.getItem('authToken');

    if (!authToken) {
        console.error('❌ No auth token found!');
        showToast('Please login again', 'error');
        setTimeout(() => window.location.reload(), 1500);
        return { 'Content-Type': 'application/json' };
    }

    return {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${authToken}`
    };
}

// API Configuration
const API_BASE_URL = 'https://localhost:7054/api';
let authToken = localStorage.getItem('authToken');
let currentUser = JSON.parse(localStorage.getItem('currentUser') || 'null');


// Delete confirmation state
let deleteConfirmData = null;

// Initialize App
document.addEventListener('DOMContentLoaded', () => {
    setTimeout(() => {
        if (typeof initializeTranslations === 'function') {
            initializeTranslations();
        }

        if (authToken && currentUser) {
            showDashboard();
        } else {
            showLogin();
        }

        // Form Submissions
        document.getElementById('loginForm')?.addEventListener('submit', handleLogin);
        document.getElementById('registerForm')?.addEventListener('submit', handleRegister);
        document.getElementById('createCategoryForm')?.addEventListener('submit', handleCreateCategory);
        document.getElementById('createMenuItemForm')?.addEventListener('submit', handleCreateMenuItem);
        document.getElementById('createTableForm')?.addEventListener('submit', handleCreateTable);
        document.getElementById('createOrderForm')?.addEventListener('submit', handleCreateOrder);
        document.getElementById('forgotPasswordForm')?.addEventListener('submit', handleForgotPassword);
        document.getElementById('resetPasswordForm')?.addEventListener('submit', handleResetPassword);
        document.getElementById('emailSettingsForm')?.addEventListener('submit', handleUpdateEmailSettings);
        document.getElementById('changePasswordForm')?.addEventListener('submit', handleChangePassword);
        document.getElementById('editProfileForm')?.addEventListener('submit', handleEditProfile);
    }, 100);
});

// ===== LOADING =====
function showLoading() {
    const overlay = document.getElementById('loadingOverlay');
    if (overlay) overlay.classList.add('show');
}

function hideLoading() {
    const overlay = document.getElementById('loadingOverlay');
    if (overlay) overlay.classList.remove('show');
}

// ===== MODALS =====
function openModal(modalId) {
    const modal = document.getElementById(modalId);
    if (modal) {
        modal.classList.add('show');
        modal.style.display = 'flex';
    }
}

function closeModal(modalId) {
    const modal = document.getElementById(modalId);
    if (modal) {
        modal.classList.remove('show');
        setTimeout(() => {
            modal.style.display = 'none';
            if (modalId === 'deleteConfirmModal' || modalId === 'editCategoryModal' || modalId === 'editMenuItemModal') {
                modal.remove();
            }
        }, 150);
    }
}

window.onclick = function (event) {
    if (event.target.classList.contains('modal')) {
        closeModal(event.target.id);
    }
}

// ===== DELETE CONFIRMATION =====
function showDeleteConfirmModal(type, id, name) {
    deleteConfirmData = { type, id, name };

    const modalHtml = `
        <div id="deleteConfirmModal" class="modal show" style="display: flex;">
            <div class="modal-content" style="max-width: 500px;">
                <span class="close" onclick="closeModal('deleteConfirmModal')">&times;</span>
                <div style="text-align: center; padding: 20px;">
                    <div style="font-size: 64px; color: #f45c43; margin-bottom: 20px;">⚠️</div>
                    <h2 style="margin-bottom: 15px;">Delete ${type}?</h2>
                    <p style="color: #a8b2d1; font-size: 16px; margin-bottom: 25px;">
                        Are you sure you want to delete <strong style="color: #fff;">${name}</strong>?
                    </p>
                    <p style="color: #f45c43; font-size: 14px; margin-bottom: 30px;">
                        This action cannot be undone!
                    </p>
                    <div style="display: flex; gap: 15px; justify-content: center;">
                        <button onclick="closeModal('deleteConfirmModal')" class="btn btn-secondary" style="min-width: 120px;">Cancel</button>
                        <button onclick="confirmDelete()" class="btn btn-danger" style="min-width: 120px;">Delete</button>
                    </div>
                </div>
            </div>
        </div>
    `;

    document.body.insertAdjacentHTML('beforeend', modalHtml);
}

async function confirmDelete() {
    if (!deleteConfirmData) return;

    const { type, id } = deleteConfirmData;
    closeModal('deleteConfirmModal');

    if (type === 'Category') await deleteCategory(id);
    else if (type === 'Menu Item') await deleteMenuItem(id);
    else if (type === 'Table') await deleteTable(id);
    else if (type === 'Order') await cancelOrder(id);

    deleteConfirmData = null;
}

// ===== TOAST =====
function showToast(message, type = 'success') {
    const toast = document.getElementById('toast');
    if (toast) {
        toast.textContent = message;
        toast.className = `toast ${type} show`;
        setTimeout(() => {
            toast.classList.remove('show');
        }, 3000);
    }
}

// ===== USER MENU =====
function toggleUserMenu(event) {
    event.stopPropagation();
    const dropdown = document.getElementById('userDropdown');
    const button = document.querySelector('.btn-user-menu');

    if (dropdown) dropdown.classList.toggle('show');
    if (button) button.classList.toggle('active');
}

document.addEventListener('click', function (event) {
    const dropdown = document.getElementById('userDropdown');
    const button = document.querySelector('.btn-user-menu');

    if (dropdown && !event.target.closest('.user-menu')) {
        dropdown.classList.remove('show');
        if (button) button.classList.remove('active');
    }
});

function goToHome() {
    window.location.href = 'index.html';
}

function goToSwagger() {
    window.open('https://localhost:7054/swagger', '_blank');
}

// ===== AUTH =====
async function handleLogin(e) {
    e.preventDefault();
    showLoading();

    const username = document.getElementById('loginUsername').value;
    const password = document.getElementById('loginPassword').value;

    try {
        const response = await fetch(`${API_BASE_URL}/auth/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ userName: username, password })
        });

        const result = await response.json();

        if (result.success) {
            authToken = result.data.token;
            currentUser = {
                userName: result.data.userName,
                fullName: result.data.fullName,
                role: result.data.role,
                email: result.data.email || ''
            };

            localStorage.setItem('authToken', authToken);
            localStorage.setItem('currentUser', JSON.stringify(currentUser));

            showToast('Login successful!', 'success');
            showDashboard();
        } else {
            showToast(result.message, 'error');
        }
    } catch (error) {
        showToast('Login failed', 'error');
    } finally {
        hideLoading();
    }
}

async function handleRegister(e) {
    e.preventDefault();
    showLoading();

    const data = {
        userName: document.getElementById('regUsername').value,
        userNameAr: document.getElementById('regUsername').value,
        email: document.getElementById('regEmail').value,
        password: document.getElementById('regPassword').value,
        fullName: document.getElementById('regFullName').value,
        role: document.getElementById('regRole').value
    };

    try {
        const response = await fetch(`${API_BASE_URL}/auth/register`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });

        const result = await response.json();

        if (result.success) {
            authToken = result.data.token;
            currentUser = {
                userName: result.data.userName,
                fullName: result.data.fullName,
                role: result.data.role,
                email: data.email
            };

            localStorage.setItem('authToken', authToken);
            localStorage.setItem('currentUser', JSON.stringify(currentUser));

            showToast('Registration successful!', 'success');
            showDashboard();
        } else {
            showToast(result.message, 'error');
        }
    } catch (error) {
        showToast('Registration failed', 'error');
    } finally {
        hideLoading();
    }
}

function logout() {
    localStorage.removeItem('authToken');
    localStorage.removeItem('currentUser');
    authToken = null;
    currentUser = null;
    showLogin();
}

// ===== NAVIGATION =====
function showLogin() {
    document.getElementById('loginSection').style.display = 'block';
    document.getElementById('registerSection').style.display = 'none';
    document.getElementById('dashboardSection').style.display = 'none';
    document.getElementById('userInfo').style.display = 'none';
    const forgot = document.getElementById('forgotPasswordSection');
    if (forgot) forgot.style.display = 'none';
}

function showRegister() {
    document.getElementById('loginSection').style.display = 'none';
    document.getElementById('registerSection').style.display = 'block';
    document.getElementById('dashboardSection').style.display = 'none';
    const forgot = document.getElementById('forgotPasswordSection');
    if (forgot) forgot.style.display = 'none';
}

function showForgotPassword() {
    document.getElementById('loginSection').style.display = 'none';
    document.getElementById('registerSection').style.display = 'none';
    document.getElementById('dashboardSection').style.display = 'none';
    const forgot = document.getElementById('forgotPasswordSection');
    if (forgot) {
        forgot.style.display = 'block';
        document.getElementById('forgotStep1').style.display = 'block';
        document.getElementById('forgotStep2').style.display = 'none';
    }
}

function showDashboard() {
    document.getElementById('loginSection').style.display = 'none';
    document.getElementById('registerSection').style.display = 'none';
    const forgot = document.getElementById('forgotPasswordSection');
    if (forgot) forgot.style.display = 'none';
    document.getElementById('dashboardSection').style.display = 'flex';
    document.getElementById('userInfo').style.display = 'flex';

    const fullName = currentUser.fullName || currentUser.userName;
    const initial = fullName.charAt(0).toUpperCase();

    document.getElementById('userName').textContent = fullName;
    document.getElementById('userInitial').textContent = initial;

    const dropdownInitial = document.getElementById('dropdownInitial');
    const dropdownName = document.getElementById('dropdownUserName');
    const dropdownRole = document.getElementById('dropdownUserRole');

    if (dropdownInitial) dropdownInitial.textContent = initial;
    if (dropdownName) dropdownName.textContent = fullName;
    if (dropdownRole) dropdownRole.textContent = currentUser.role || 'User';

    loadCategories();
    loadMenuItems();
    loadTables();
    loadOrders();
}

function showTab(tabName) {
    document.querySelectorAll('.nav-btn').forEach(btn => btn.classList.remove('active'));
    document.querySelectorAll('.tab-content').forEach(tab => tab.classList.remove('active'));

    const navBtn = document.querySelector(`.nav-btn[data-tab="${tabName}"]`);
    if (navBtn) navBtn.classList.add('active');

    setTimeout(() => {
        const targetTab = document.getElementById(tabName + 'Tab');
        if (targetTab) targetTab.classList.add('active');

        if (tabName === 'categories') loadCategories();
        if (tabName === 'menuItems') loadMenuItems();
        if (tabName === 'tables') loadTables();
        if (tabName === 'orders') loadOrders();
    }, 200);
}

// ===== CREATE CATEGORY =====
async function handleCreateCategory(e) {
    e.preventDefault();
    showLoading();

    const data = {
        categoryName: document.getElementById('catName').value.trim(),
        categoryNameAr: document.getElementById('catNameAr').value.trim() || "",
        description: document.getElementById('catDescription').value.trim() || "",
        displayOrder: parseInt(document.getElementById('catDisplayOrder').value) || 1
    };

    try {
        const response = await fetch(`${API_BASE_URL}/menu/categories`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${authToken}`
            },
            body: JSON.stringify(data)
        });

        const result = await response.json();

        if (result.success) {
            showToast('✅ Category created!', 'success');
            closeModal('createCategoryModal');
            document.getElementById('createCategoryForm').reset();
            setTimeout(() => loadCategories(), 500);
        } else {
            showToast(result.message || 'Failed', 'error');
        }
    } catch (error) {
        console.error('Error:', error);
        showToast('Error creating category', 'error');
    } finally {
        hideLoading();
    }
}

// ===== CREATE MENU ITEM =====
async function handleCreateMenuItem(e) {
    e.preventDefault();
    showLoading();

    const data = {
        categoryId: parseInt(document.getElementById('menuItemCategory').value),
        name: document.getElementById('menuItemName').value,
        nameAr: document.getElementById('menuItemNameAr').value,
        description: document.getElementById('menuItemDescription').value,
        price: parseFloat(document.getElementById('menuItemPrice').value),
        imageUrl: document.getElementById('menuItemImageUrl')?.value || "",
        preparationTime: parseInt(document.getElementById('menuItemPrepTime').value),
        isAvailable: document.getElementById('menuItemAvailable')?.checked !== false
    };

    try {
        const response = await fetch(`${API_BASE_URL}/menu/items`, {
            method: 'POST',
            headers: getAuthHeaders(),
            body: JSON.stringify(data)
        });

        const result = await response.json();

        if (result.success) {
            showToast('✅ Menu item created!', 'success');
            closeModal('createMenuItemModal');
            document.getElementById('createMenuItemForm').reset();
            setTimeout(() => loadMenuItems(), 500);
        } else {
            showToast(result.message, 'error');
        }
    } catch (error) {
        console.error('Error:', error);
        showToast('Error creating item', 'error');
    } finally {
        hideLoading();
    }
}

// ===== EDIT MENU ITEM =====
async function handleEditMenuItem(e, id) {
    e.preventDefault();
    showLoading();

    const data = {
        categoryId: parseInt(document.getElementById('editItemCategory').value),
        name: document.getElementById('editItemName').value,
        nameAr: document.getElementById('editItemNameAr').value,
        description: document.getElementById('editItemDescription').value,
        price: parseFloat(document.getElementById('editItemPrice').value),
        imageUrl: document.getElementById('editItemImageUrl').value,
        preparationTime: parseInt(document.getElementById('editItemPrepTime').value),
        isAvailable: document.getElementById('editItemAvailable')?.checked !== false
    };

    try {
        const response = await fetch(`${API_BASE_URL}/menu/items/${id}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${authToken}`
            },
            body: JSON.stringify(data)
        });

        const result = await response.json();

        if (result.success) {
            showToast('✅ Item updated!', 'success');
            closeModal('editMenuItemModal');
            setTimeout(() => loadMenuItems(), 500);
        } else {
            showToast(result.message, 'error');
        }
    } catch (error) {
        console.error('Error:', error);
        showToast('Error updating item', 'error');
    } finally {
        hideLoading();
    }
}

// ===== DELETE TABLE =====
async function deleteTable(id) {
    showLoading();
    try {
        const response = await fetch(`${API_BASE_URL}/table/${id}`, {
            method: 'DELETE',
            headers: getAuthHeaders()
        });

        if (response.status === 400) {
            showToast('Cannot delete table with active orders!', 'error');
            hideLoading();
            return;
        }

        const result = await response.json();

        if (result.success) {
            showToast('✅ Table deleted!', 'success');
            setTimeout(() => loadTables(), 500);
        } else {
            showToast(result.message, 'error');
        }
    } catch (error) {
        console.error('Error:', error);
        showToast('Cannot delete table', 'error');
    } finally {
        hideLoading();
    }
}

// ===== LOAD ORDERS WITH FULL DATA =====
async function loadOrders() {
    showLoading();
    try {
        const response = await fetch(`${API_BASE_URL}/order?page=1&pageSize=100`, {
            headers: { 'Authorization': `Bearer ${authToken}` }
        });
        const result = await response.json();

        console.log('📦 Orders:', result);

        if (result.success) {
            const orders = result.data.items || result.data;
            displayOrders(orders);
        }
    } catch (error) {
        console.error('Error:', error);
        showToast('Error loading orders', 'error');
    } finally {
        hideLoading();
    }
}

function displayOrders(orders) {
    const container = document.getElementById('ordersList');

    if (orders.length === 0) {
        container.innerHTML = `<div class="card"><p style="color: white; text-align: center; padding: 40px;">No orders found</p></div>`;
        return;
    }

    let html = `
        <div class="card">
            <table class="data-table">
                <thead>
                    <tr>
                        <th>Order Number</th>
                        <th>Table</th>
                        <th>Waiter</th>
                        <th>Items</th>
                        <th>Total (EGP)</th>
                        <th>Status</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
    `;

    orders.forEach(order => {
        const statusClass = `status-${order.status.toLowerCase()}`;

        // Show items with names
        let itemsList = '-';
        if (order.items && order.items.length > 0) {
            itemsList = order.items.map(item => {
                const name = item.menuItemName || item.name || `Item #${item.menuItemId}`;
                return `${item.quantity}x ${name}`;
            }).join(', ');
        }

        let actionButtons = '';
        if (order.status === 'Pending') {
            actionButtons = `<button class="btn btn-warning btn-sm" onclick="updateOrderStatus(${order.id}, 'Preparing')">🔥 Prepare</button>`;
        } else if (order.status === 'Preparing') {
            actionButtons = `<button class="btn btn-warning btn-sm" onclick="updateOrderStatus(${order.id}, 'Ready')">✅ Ready</button>`;
        } else if (order.status === 'Ready') {
            actionButtons = `<button class="btn btn-success btn-sm" onclick="updateOrderStatus(${order.id}, 'Served')">🍽️ Served</button>`;
        }

        actionButtons += ` <button class="btn btn-danger btn-sm" onclick="showDeleteConfirmModal('Order', ${order.id}, 'Order #${order.orderNumber}')">🗑️ Cancel</button>`;

        html += `
            <tr>
                <td><strong>#${order.orderNumber}</strong></td>
                <td>Table ${order.tableNumber || 'N/A'}</td>
                <td>${order.userName || 'N/A'}</td>
                <td style="max-width: 300px; white-space: normal;"><strong>${itemsList}</strong></td>
                <td class="price-cell">${order.totalAmount ? order.totalAmount.toFixed(2) : '0.00'} EGP</td>
                <td><span class="status-badge ${statusClass}">${order.status}</span></td>
                <td class="action-cell">${actionButtons}</td>
            </tr>
        `;
    });

    html += `</tbody></table></div>`;
    container.innerHTML = html;
}

// ===== DELETE ORDER =====
async function cancelOrder(id) {
    showLoading();
    try {
        const response = await fetch(`${API_BASE_URL}/order/${id}`, {
            method: 'DELETE',
            headers: { 'Authorization': `Bearer ${authToken}` }
        });

        const result = await response.json();

        if (result.success) {
            showToast('✅ Order cancelled!', 'success');
            setTimeout(() => {
                loadOrders();
                loadTables();
            }, 500);
        } else {
            showToast(result.message, 'error');
        }
    } catch (error) {
        console.error('Error:', error);
        showToast('Error cancelling order', 'error');
    } finally {
        hideLoading();
    }
}

async function updateOrderStatus(orderId, newStatus) {
    showLoading();
    try {
        const response = await fetch(`${API_BASE_URL}/order/${orderId}/status`, {
            method: 'PATCH',
            headers: getAuthHeaders(),
            body: JSON.stringify({ status: newStatus })
        });

        if (response.status === 401) {
            showToast('Session expired. Please login again.', 'error');
            localStorage.removeItem('authToken');
            localStorage.removeItem('currentUser');
            setTimeout(() => window.location.reload(), 1500);
            return;
        }

        const result = await response.json();
        if (result.success) {
            showToast('✅ Status updated!', 'success');
            setTimeout(() => {
                loadOrders();
                loadTables();
            }, 500);
        } else {
            showToast(result.message, 'error');
        }
    } catch (error) {
        showToast('Error updating status', 'error');
    } finally {
        hideLoading();
    }
}


async function loadMenuItems() {
    showLoading();
    try {
        const response = await fetch(`${API_BASE_URL}/menu/items?page=1&pageSize=100`, {
            headers: { 'Authorization': `Bearer ${authToken}` }
        });
        const result = await response.json();

        console.log('📦 Items:', result);

        if (result.success) {
            displayMenuItems(result.data.items || result.data);
        }
    } catch (error) {
        console.error('Error:', error);
        showToast('Error loading items', 'error');
    } finally {
        hideLoading();
    }
}

function displayMenuItems(items) {
    const container = document.getElementById('menuItemsList');

    if (items.length === 0) {
        container.innerHTML = `<div class="card"><p style="color: white; text-align: center; padding: 40px;">No menu items found</p></div>`;
        return;
    }

    let html = `
        <div class="card">
            <table class="data-table">
                <thead>
                    <tr>
                        <th>Item Name</th>
                        <th>Arabic Name</th>
                        <th>Category</th>
                        <th>Price (EGP)</th>
                        <th>Prep Time</th>
                        <th>Status</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
    `;

    items.forEach(item => {
        const availableClass = item.isAvailable ? 'status-available' : 'status-unavailable';
        const availableText = item.isAvailable ? '✅ Available' : '❌ Unavailable';

        html += `
            <tr>
                <td><strong>${item.name}</strong></td>
                <td>${item.nameAr || '-'}</td>
                <td>${item.categoryName || 'N/A'}</td>
                <td class="price-cell">${item.price.toFixed(2)} EGP</td>
                <td>${item.preparationTime || 0} mins</td>
                <td><span class="status-badge ${availableClass}" id="status-badge-${item.id}">${availableText}</span></td>
                <td class="action-cell">
                    <button class="btn btn-info btn-sm" onclick="showEditMenuItemModal(${item.id})">✏️ Edit</button>
                    <button class="btn btn-warning btn-sm" onclick="toggleAvailability(${item.id})">🔄 Toggle</button>
                    <button class="btn btn-danger btn-sm" onclick="showDeleteConfirmModal('Menu Item', ${item.id}, '${item.name.replace(/'/g, "\\'")}')">🗑️ Delete</button>
                </td>
            </tr>
        `;
    });

    html += `</tbody></table></div>`;
    container.innerHTML = html;
}

async function toggleAvailability(id) {
    showLoading();
    try {
        const response = await fetch(`${API_BASE_URL}/menu/items/${id}/toggle-availability`, {
            method: 'PATCH',
            headers: getAuthHeaders()
        });

        if (response.status === 401) {
            showToast('Session expired. Please login again.', 'error');
            setTimeout(() => window.location.reload(), 1500);
            return;
        }

        const result = await response.json();

        if (result.success) {
            // Get fresh data
            const itemResponse = await fetch(`${API_BASE_URL}/menu/items/${id}`, {
                headers: getAuthHeaders()
            });

            const itemResult = await itemResponse.json();

            if (itemResult.success) {
                const isAvailable = itemResult.data.isAvailable;
                const badge = document.getElementById(`status-badge-${id}`);

                if (badge) {
                    badge.className = `status-badge ${isAvailable ? 'status-available' : 'status-unavailable'}`;
                    badge.textContent = isAvailable ? '✅ Available' : '❌ Unavailable';
                }

                showToast(`Item is now ${isAvailable ? 'Available' : 'Unavailable'}`, 'success');
            }
        } else {
            showToast(result.message, 'error');
        }
    } catch (error) {
        console.error('Error:', error);
        showToast('Failed to toggle', 'error');
    } finally {
        hideLoading();
    }
}

async function deleteMenuItem(id) {
    showLoading();
    try {
        const response = await fetch(`${API_BASE_URL}/menu/items/${id}`, {
            method: 'DELETE',
            headers: { 'Authorization': `Bearer ${authToken}` }
        });
        const result = await response.json();
        if (result.success) {
            showToast('✅ Item deleted!', 'success');
            setTimeout(() => loadMenuItems(), 500);
        } else {
            showToast(result.message, 'error');
        }
    } catch (error) {
        showToast('Error deleting item', 'error');
    } finally {
        hideLoading();
    }
}

// ===== LOAD CATEGORIES =====
async function loadCategories() {
    showLoading();
    try {
        const [categoriesRes, itemsRes] = await Promise.all([
            fetch(`${API_BASE_URL}/menu/categories`, {
                headers: { 'Authorization': `Bearer ${authToken}` }
            }),
            fetch(`${API_BASE_URL}/menu/items?page=1&pageSize=1000`, {
                headers: { 'Authorization': `Bearer ${authToken}` }
            })
        ]);

        const categoriesResult = await categoriesRes.json();
        const itemsResult = await itemsRes.json();

        if (categoriesResult.success) {
            const allItems = itemsResult.data?.items || itemsResult.data || [];
            const categories = categoriesResult.data.map(cat => {
                const categoryItems = allItems.filter(item => item.categoryId === cat.id);
                return { ...cat, items: categoryItems };
            });

            displayCategories(categories);
        }
    } catch (error) {
        showToast('Error loading categories', 'error');
    } finally {
        hideLoading();
    }
}

function displayCategories(categories) {
    const container = document.getElementById('categoriesList');

    if (categories.length === 0) {
        container.innerHTML = `<div class="card"><p style="color: white; text-align: center; padding: 40px;">No categories found</p></div>`;
        return;
    }

    let html = `
        <div class="card">
            <table class="data-table">
                <thead>
                    <tr>
                        <th>Category Name</th>
                        <th>Arabic Name</th>
                        <th>Description</th>
                        <th>Items</th>
                        <th>Display Order</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
    `;

    categories.forEach(cat => {
        let itemsList = '';
        if (cat.items && cat.items.length > 0) {
            itemsList = cat.items.map(item => item.name).join(', ');
        } else {
            itemsList = '-';
        }

        const itemCount = cat.items ? cat.items.length : 0;

        html += `
            <tr>
                <td><strong>${cat.categoryName}</strong></td>
                <td>${cat.categoryNameAr || '-'}</td>
                <td>${cat.description || '-'}</td>
                <td>
                    <span class="item-count-badge">${itemCount}</span>
                    ${itemsList !== '-' ? `<div class="items-list">${itemsList}</div>` : ''}
                </td>
                <td>${cat.displayOrder}</td>
                <td class="action-cell">
                    <button class="btn btn-info btn-sm" onclick="showEditCategoryModal(${cat.id})">✏️ Edit</button>
                    <button class="btn btn-danger btn-sm" onclick="showDeleteConfirmModal('Category', ${cat.id}, '${cat.categoryName}')">🗑️ Delete</button>
                </td>
            </tr>
        `;
    });

    html += `</tbody></table></div>`;
    container.innerHTML = html;
}

async function handleEditCategory(e, id) {
    e.preventDefault();
    showLoading();

    const data = {
        categoryName: document.getElementById('editCatName').value,
        categoryNameAr: document.getElementById('editCatNameAr').value,
        description: document.getElementById('editCatDescription').value,
        displayOrder: parseInt(document.getElementById('editCatDisplayOrder').value)
    };

    try {
        const response = await fetch(`${API_BASE_URL}/menu/categories/${id}`, {
            method: 'PUT',
            headers: getAuthHeaders(),
            body: JSON.stringify(data)
        });

        if (response.status === 401) {
            showToast('Session expired. Please login again.', 'error');
            localStorage.removeItem('authToken');
            localStorage.removeItem('currentUser');
            setTimeout(() => window.location.reload(), 1500);
            return;
        }

        const result = await response.json();

        if (result.success) {
            showToast('✅ Category updated!', 'success');
            closeModal('editCategoryModal');
            setTimeout(() => loadCategories(), 500);
        } else {
            showToast(result.message, 'error');
        }
    } catch (error) {
        showToast('Error updating category', 'error');
    } finally {
        hideLoading();
    }
}

async function deleteCategory(id) {
    showLoading();
    try {
        const response = await fetch(`${API_BASE_URL}/menu/categories/${id}`, {
            method: 'DELETE',
            headers: { 'Authorization': `Bearer ${authToken}` }
        });
        const result = await response.json();
        if (result.success) {
            showToast('✅ Category deleted!', 'success');
            setTimeout(() => loadCategories(), 500);
        } else {
            showToast(result.message, 'error');
        }
    } catch (error) {
        showToast('Error deleting category', 'error');
    } finally {
        hideLoading();
    }
}

// ===== LOAD TABLES =====
async function loadTables() {
    showLoading();
    try {
        const response = await fetch(`${API_BASE_URL}/table`, {
            headers: getAuthHeaders()
        });

        if (response.status === 401) {
            showToast('Session expired. Please login again.', 'error');
            localStorage.removeItem('authToken');
            localStorage.removeItem('currentUser');
            setTimeout(() => window.location.reload(), 1500);
            return;
        }

        const result = await response.json();

        console.log('📦 Tables:', result);

        if (result.success) {
            displayTables(result.data);
        }
    } catch (error) {
        showToast('Error loading tables', 'error');
    } finally {
        hideLoading();
    }
}
function displayTables(tables) {
    const container = document.getElementById('tablesList');

    if (tables.length === 0) {
        container.innerHTML = `<div class="card"><p style="color: white; text-align: center; padding: 40px;">No tables found</p></div>`;
        return;
    }

    let html = `
        <div class="card">
            <table class="data-table">
                <thead>
                    <tr>
                        <th>Table Number</th>
                        <th>Capacity</th>
                        <th>Floor Section</th>
                        <th>Status</th>
                        <th>Current Order</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
    `;

    tables.forEach(table => {
        const statusClass = `status-${table.status.toLowerCase()}`;

        html += `
            <tr>
                <td><strong>Table #${table.tableNumber}</strong></td>
                <td>${table.capacity} people</td>
                <td>${table.floorSection}</td>
                <td><span class="status-badge ${statusClass}">${table.status}</span></td>
                <td>${table.currentOrderNumber || '-'}</td>
                <td class="action-cell">
                    <button class="btn btn-danger btn-sm" onclick="showDeleteConfirmModal('Table', ${table.id}, 'Table #${table.tableNumber}')">🗑️ Delete</button>
                </td>
            </tr>
        `;
    });

    html += `</tbody></table></div>`;
    container.innerHTML = html;
}

async function handleCreateTable(e) {
    e.preventDefault();
    showLoading();

    const data = {
        tableNumber: parseInt(document.getElementById('tableNumber').value),
        capacity: parseInt(document.getElementById('tableCapacity').value),
        floorSection: document.getElementById('tableSection').value
    };

    try {
        const response = await fetch(`${API_BASE_URL}/table`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${authToken}`
            },
            body: JSON.stringify(data)
        });
        const result = await response.json();
        if (result.success) {
            showToast('✅ Table created!', 'success');
            closeModal('createTableModal');
            document.getElementById('createTableForm').reset();
            setTimeout(() => loadTables(), 500);
        } else {
            showToast(result.message, 'error');
        }
    } catch (error) {
        showToast('Error creating table', 'error');
    } finally {
        hideLoading();
    }
}

// ===== CREATE ORDER =====
async function handleCreateOrder(e) {
    e.preventDefault();
    showLoading();

    const items = [];
    document.querySelectorAll('.order-item-row').forEach(row => {
        const menuItemId = parseInt(row.querySelector('.orderItemSelect').value);
        const quantity = parseInt(row.querySelector('.orderItemQty').value);
        if (menuItemId && quantity > 0) {
            items.push({ menuItemId, quantity, specialRequest: "" });
        }
    });

    if (items.length === 0) {
        hideLoading();
        showToast('Please add at least one item', 'error');
        return;
    }

    const data = {
        tableId: parseInt(document.getElementById('orderTable').value),
        userId: 1,
        notes: document.getElementById('orderNotes').value,
        items
    };

    try {
        const response = await fetch(`${API_BASE_URL}/order`, {
            method: 'POST',
            headers: getAuthHeaders(),
            body: JSON.stringify(data)
        });
        const result = await response.json();
        if (result.success) {
            showToast('✅ Order created!', 'success');
            closeModal('createOrderModal');
            setTimeout(() => {
                loadOrders();
                loadTables();
            }, 500);
        } else {
            showToast(result.message, 'error');
        }
    } catch (error) {
        showToast('Error creating order', 'error');
    } finally {
        hideLoading();
    }
}

async function showCreateMenuItemModal() {
    showLoading();
    try {
        const response = await fetch(`${API_BASE_URL}/menu/categories`, {
            headers: { 'Authorization': `Bearer ${authToken}` }
        });
        const result = await response.json();
        if (result.success) {
            const select = document.getElementById('menuItemCategory');
            select.innerHTML = result.data.map(cat =>
                `<option value="${cat.id}">${cat.categoryName}</option>`
            ).join('');
            openModal('createMenuItemModal');
        }
    } catch (error) {
        showToast('Failed to load categories', 'error');
    } finally {
        hideLoading();
    }
}

async function showCreateOrderModal() {
    showLoading();
    try {
        const [tablesRes, itemsRes] = await Promise.all([
            fetch(`${API_BASE_URL}/table/available`, {
                headers: { 'Authorization': `Bearer ${authToken}` }
            }),
            fetch(`${API_BASE_URL}/menu/items?page=1&pageSize=100`, {
                headers: { 'Authorization': `Bearer ${authToken}` }
            })
        ]);

        const tablesResult = await tablesRes.json();
        const itemsResult = await itemsRes.json();

        if (tablesResult.success && itemsResult.success) {
            const tableSelect = document.getElementById('orderTable');

            if (tablesResult.data && tablesResult.data.length > 0) {
                tableSelect.innerHTML = tablesResult.data.map(t =>
                    `<option value="${t.id}">Table #${t.tableNumber} (${t.capacity} seats)</option>`
                ).join('');
            } else {
                tableSelect.innerHTML = '<option value="">No available tables</option>';
            }

            availableMenuItems = itemsResult.data.items.filter(item => item.isAvailable);
            orderItemCount = 0;
            document.getElementById('orderItemsList').innerHTML = '';
            addOrderItem();
            openModal('createOrderModal');
        }
    } catch (error) {
        showToast('Failed to load order data', 'error');
    } finally {
        hideLoading();
    }
}

let orderItemCount = 0;
let availableMenuItems = [];

function addOrderItem() {
    orderItemCount++;
    const container = document.getElementById('orderItemsList');
    const div = document.createElement('div');
    div.className = 'order-item-row';
    div.id = `orderItem${orderItemCount}`;
    div.innerHTML = `
        <div class="form-group">
            <select class="orderItemSelect" required>
                <option value="">Select Item</option>
                ${availableMenuItems.map(item =>
        `<option value="${item.id}">${item.name} - ${(item.price || 0).toFixed(2)} EGP</option>`
    ).join('')}
            </select>
        </div>
        <div class="form-group">
            <input type="number" class="orderItemQty" placeholder="Qty" min="1" value="1" required>
        </div>
        <button type="button" class="btn btn-danger" onclick="removeOrderItem(${orderItemCount})">Remove</button>
    `;
    container.appendChild(div);
}

function removeOrderItem(id) {
    const element = document.getElementById(`orderItem${id}`);
    if (element) element.remove();
}

// ===== MODALS =====
async function showEditCategoryModal(id) {
    showLoading();
    try {
        const response = await fetch(`${API_BASE_URL}/menu/categories/${id}`, {
            headers: { 'Authorization': `Bearer ${authToken}` }
        });
        const result = await response.json();
        if (result.success) {
            const cat = result.data;
            const modalHtml = `
                <div id="editCategoryModal" class="modal show" style="display: flex;">
                    <div class="modal-content">
                        <span class="close" onclick="closeModal('editCategoryModal')">&times;</span>
                        <h2>Edit Category</h2>
                        <form id="editCategoryForm" onsubmit="handleEditCategory(event, ${cat.id}); return false;">
                            <div class="form-group">
                                <label>Category Name</label>
                                <input type="text" id="editCatName" value="${cat.categoryName || ''}" required>
                            </div>
                            <div class="form-group">
                                <label>Category Name (Arabic)</label>
                                <input type="text" id="editCatNameAr" value="${cat.categoryNameAr || ''}" required>
                            </div>
                            <div class="form-group">
                                <label>Description</label>
                                <textarea id="editCatDescription" rows="3">${cat.description || ''}</textarea>
                            </div>
                            <div class="form-group">
                                <label>Display Order</label>
                                <input type="number" id="editCatDisplayOrder" value="${cat.displayOrder || 1}" required>
                            </div>
                            <button type="submit" class="btn btn-primary">Update</button>
                        </form>
                    </div>
                </div>
            `;
            document.body.insertAdjacentHTML('beforeend', modalHtml);
        }
    } catch (error) {
        showToast('Failed to load category', 'error');
    } finally {
        hideLoading();
    }
}

async function showEditMenuItemModal(id) {
    showLoading();
    try {
        const [categoriesRes, itemRes] = await Promise.all([
            fetch(`${API_BASE_URL}/menu/categories`, {
                headers: { 'Authorization': `Bearer ${authToken}` }
            }),
            fetch(`${API_BASE_URL}/menu/items/${id}`, {
                headers: { 'Authorization': `Bearer ${authToken}` }
            })
        ]);
        const categoriesResult = await categoriesRes.json();
        const itemResult = await itemRes.json();
        if (categoriesResult.success && itemResult.success) {
            const item = itemResult.data;
            const categoriesOptions = categoriesResult.data.map(cat =>
                `<option value="${cat.id}" ${cat.id === item.categoryId ? 'selected' : ''}>${cat.categoryName}</option>`
            ).join('');

            const modalHtml = `
                <div id="editMenuItemModal" class="modal show" style="display: flex;">
                    <div class="modal-content">
                        <span class="close" onclick="closeModal('editMenuItemModal')">&times;</span>
                        <h2>Edit Menu Item</h2>
                        <form id="editMenuItemForm" onsubmit="handleEditMenuItem(event, ${item.id}); return false;">
                            <div class="form-group">
                                <label>Category</label>
                                <select id="editItemCategory" required>${categoriesOptions}</select>
                            </div>
                            <div class="form-group">
                                <label>Item Name</label>
                                <input type="text" id="editItemName" value="${item.name || ''}" required>
                            </div>
                            <div class="form-group">
                                <label>Item Name (Arabic)</label>
                                <input type="text" id="editItemNameAr" value="${item.nameAr || ''}" required>
                            </div>
                            <div class="form-group">
                                <label>Description</label>
                                <textarea id="editItemDescription" rows="3">${item.description || ''}</textarea>
                            </div>
                            <div class="form-group">
                                <label>Price (EGP)</label>
                                <input type="number" step="0.01" id="editItemPrice" value="${item.price || 0}" required>
                            </div>
                            <div class="form-group">
                                <label>Image URL</label>
                                <input type="text" id="editItemImageUrl" value="${item.imageUrl || ''}" placeholder="https://example.com/image.jpg">
                            </div>
                            <div class="form-group">
                                <label>Preparation Time (minutes)</label>
                                <input type="number" id="editItemPrepTime" value="${item.preparationTime || 10}" required>
                            </div>
                            <div class="checkbox-wrapper">
                                <input type="checkbox" id="editItemAvailable" ${item.isAvailable ? 'checked' : ''}>
                                <label for="editItemAvailable">Available for Order</label>
                            </div>
                            <button type="submit" class="btn btn-primary">Update</button>
                        </form>
                    </div>
                </div>
            `;
            document.body.insertAdjacentHTML('beforeend', modalHtml);
        }
    } catch (error) {
        showToast('Failed to load menu item', 'error');
    } finally {
        hideLoading();
    }
}

// ===== PROFILE =====
function showEditProfileModal() {
    const dropdown = document.getElementById('userDropdown');
    if (dropdown) dropdown.classList.remove('show');

    const user = JSON.parse(localStorage.getItem('currentUser') || '{}');

    document.getElementById('editFullName').value = user.fullName || '';
    document.getElementById('editEmail').value = user.email || '';
    document.getElementById('editUsername').value = user.userName || '';

    openModal('editProfileModal');
}

async function handleEditProfile(e) {
    e.preventDefault();
    showLoading();

    const data = {
        fullName: document.getElementById('editFullName').value,
        email: document.getElementById('editEmail').value
    };

    try {
        showToast('Profile updated!', 'success');
        currentUser.fullName = data.fullName;
        currentUser.email = data.email;
        localStorage.setItem('currentUser', JSON.stringify(currentUser));
        closeModal('editProfileModal');
        showDashboard();
    } catch (error) {
        showToast('Failed to update profile', 'error');
    } finally {
        hideLoading();
    }
}

function showChangePasswordModal() {
    const dropdown = document.getElementById('userDropdown');
    if (dropdown) dropdown.classList.remove('show');
    openModal('changePasswordModal');
}

async function handleChangePassword(e) {
    e.preventDefault();
    showLoading();

    const currentPassword = document.getElementById('currentPassword').value;
    const newPassword = document.getElementById('newPasswordChange').value;
    const confirmPassword = document.getElementById('confirmPasswordChange').value;

    if (newPassword !== confirmPassword) {
        hideLoading();
        showToast('Passwords do not match!', 'error');
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}/auth/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                userName: currentUser.userName,
                password: currentPassword
            })
        });
        const result = await response.json();
        if (result.success) {
            showToast('Password changed!', 'success');
            closeModal('changePasswordModal');
            document.getElementById('changePasswordForm').reset();
        } else {
            showToast('Current password is incorrect', 'error');
        }
    } catch (error) {
        showToast('Failed to change password', 'error');
    } finally {
        hideLoading();
    }
}

// ===== OTHER FUNCTIONS =====
async function loadEmailSettings() {
    try {
        const response = await fetch(`${API_BASE_URL}/Settings/email`, {
            headers: { 'Authorization': `Bearer ${authToken}` }
        });
        const result = await response.json();
        if (result.success && result.data) {
            const settings = result.data;
            document.getElementById('settingSmtpServer').value = settings.smtpServer || '';
            document.getElementById('settingSmtpPort').value = settings.smtpPort || '';
            document.getElementById('settingFromEmail').value = settings.fromEmail || '';
            document.getElementById('settingPassword').value = settings.password || '';
            document.getElementById('settingEnableSsl').checked = settings.enableSsl;
        }
    } catch (error) {
        console.error('Failed to load email settings');
    }
}

async function handleUpdateEmailSettings(e) {
    e.preventDefault();
    showLoading();

    const data = {
        smtpServer: document.getElementById('settingSmtpServer').value,
        smtpPort: document.getElementById('settingSmtpPort').value,
        fromEmail: document.getElementById('settingFromEmail').value,
        password: document.getElementById('settingPassword').value,
        enableSsl: document.getElementById('settingEnableSsl').checked
    };

    try {
        const response = await fetch(`${API_BASE_URL}/settings/email`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${authToken}`
            },
            body: JSON.stringify(data)
        });
        const result = await response.json();
        if (result.success) {
            showToast('Email settings updated!', 'success');
        } else {
            showToast(result.message, 'error');
        }
    } catch (error) {
        showToast('Failed to update settings', 'error');
    } finally {
        hideLoading();
    }
}

async function handleForgotPassword(e) {
    e.preventDefault();
    showLoading();

    const email = document.getElementById('forgotEmail').value;

    try {
        const response = await fetch(`${API_BASE_URL}/auth/forgot-password`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email })
        });

        const result = await response.json();

        if (result.success) {
            showToast('OTP sent to your email!', 'success');
            document.getElementById('forgotStep1').style.display = 'none';
            document.getElementById('forgotStep2').style.display = 'block';
        } else {
            showToast(result.message || 'Failed to send OTP', 'error');
        }
    } catch (error) {
        showToast('Failed to send OTP', 'error');
    } finally {
        hideLoading();
    }
}

async function handleResetPassword(e) {
    e.preventDefault();
    showLoading();

    const email = document.getElementById('forgotEmail').value;
    const otp = document.getElementById('otpCode').value;
    const newPassword = document.getElementById('newPassword').value;
    const confirmPassword = document.getElementById('confirmPasswordReset').value;

    if (newPassword !== confirmPassword) {
        hideLoading();
        showToast('Passwords do not match!', 'error');
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}/Auth/reset-password`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, otp, newPassword })
        });
        const result = await response.json();
        if (result.success) {
            showToast('Password reset successfully!', 'success');
            showLogin();
        } else {
            showToast(result.message, 'error');
        }
    } catch (error) {
        showToast('Failed to reset password', 'error');
    } finally {
        hideLoading();
    }
}