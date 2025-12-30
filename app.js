// API Configuration
const API_BASE_URL = 'https://localhost:7054/api';
let authToken = localStorage.getItem('authToken');
let currentUser = JSON.parse(localStorage.getItem('currentUser') || 'null');

// Delete confirmation state
let deleteConfirmData = null;

// Initialize App - WAIT for translations to load
document.addEventListener('DOMContentLoaded', () => {
    // Wait a bit for translations.js to fully load
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
            if (modalId === 'deleteConfirmModal') {
                modal.remove();
            }
        }, 150);
    }
}

// Close modal on background click
window.onclick = function (event) {
    if (event.target.classList.contains('modal')) {
        closeModal(event.target.id);
    }
}

// ===== DELETE CONFIRMATION MODAL =====
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
                        <button onclick="closeModal('deleteConfirmModal')" class="btn btn-secondary" style="min-width: 120px;">
                            Cancel
                        </button>
                        <button onclick="confirmDelete()" class="btn btn-danger" style="min-width: 120px;">
                            Delete
                        </button>
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
    toast.textContent = message;
    toast.className = `toast ${type} show`;
    setTimeout(() => {
        toast.classList.remove('show');
    }, 3000);
}

// ===== USER MENU =====
function toggleUserMenu(event) {
    event.stopPropagation();
    const dropdown = document.getElementById('userDropdown');
    const button = document.querySelector('.btn-user-menu');
    dropdown?.classList.toggle('show');
    button?.classList.toggle('active');
}

document.addEventListener('click', function (event) {
    const dropdown = document.getElementById('userDropdown');
    const button = document.querySelector('.btn-user-menu');
    if (dropdown && !event.target.closest('.user-menu')) {
        dropdown.classList.remove('show');
        button?.classList.remove('active');
    }
});

function goToHome() {
    window.location.href = 'index.html';
}

function goToSwagger() {
    window.open('https://localhost:7054/swagger', '_blank');
}

function showSettingsTab() {
    const dropdown = document.getElementById('userDropdown');
    dropdown?.classList.remove('show');

    document.querySelectorAll('.tab-content').forEach(tab => tab.classList.remove('active'));
    document.querySelectorAll('.tab-btn').forEach(btn => btn.classList.remove('active'));

    document.getElementById('settingsTab')?.classList.add('active');
    loadEmailSettings();
}

// ===== AUTH =====
async function handleLogin(e) {
    e.preventDefault();
    showLoading();

    const username = document.getElementById('loginUsername').value;
    const password = document.getElementById('loginPassword').value;

    try {
        const response = await fetch(`${API_BASE_URL}/Auth/login`, {
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

            showToast(t('loginSuccess'), 'success');
            showDashboard();
        } else {
            showToast(result.message, 'error');
        }
    } catch (error) {
        showToast(t('loginFailed'), 'error');
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
        const response = await fetch(`${API_BASE_URL}/Auth/register`, {
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

            showToast(t('registerSuccess'), 'success');
            showDashboard();
        } else {
            showToast(result.message, 'error');
        }
    } catch (error) {
        showToast(t('registerFailed'), 'error');
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
    document.getElementById('dashboardSection').style.display = 'block';
    document.getElementById('userInfo').style.display = 'flex';

    const fullName = currentUser.fullName || currentUser.userName;
    const initial = fullName.charAt(0).toUpperCase();

    document.getElementById('userName').textContent = fullName;
    document.getElementById('userInitial').textContent = initial;

    const dropdownInitial = document.getElementById('dropdownInitial');
    const dropdownName = document.getElementById('dropdownUserName');
    const dropdownRole = document.getElementById('dropdownUserRole');
    const settingsMenuBtn = document.getElementById('settingsMenuBtn');

    if (dropdownInitial) dropdownInitial.textContent = initial;
    if (dropdownName) dropdownName.textContent = fullName;
    if (dropdownRole) dropdownRole.textContent = currentUser.role || 'User';

    if (settingsMenuBtn) {
        settingsMenuBtn.style.display = currentUser.role === 'Admin' ? 'block' : 'none';
    }

    loadCategories();
    loadMenuItems();
    loadTables();
    loadOrders();

    if (currentUser.role === 'Admin') {
        loadEmailSettings();
    }
}

function showTab(tabName) {
    document.querySelectorAll('.tab-content').forEach(tab => tab.classList.remove('active'));
    document.querySelectorAll('.tab-btn').forEach(btn => btn.classList.remove('active'));

    document.getElementById(tabName + 'Tab')?.classList.add('active');
    event.target.classList.add('active');

    if (tabName === 'categories') loadCategories();
    if (tabName === 'menuItems') loadMenuItems();
    if (tabName === 'tables') loadTables();
    if (tabName === 'orders') loadOrders();
}

// ===== CATEGORIES =====
async function loadCategories() {
    showLoading();
    try {
        const response = await fetch(`${API_BASE_URL}/Menu/categories`, {
            headers: { 'Authorization': `Bearer ${authToken}` }
        });
        const result = await response.json();
        if (result.success) {
            displayCategories(result.data);
        }
    } catch (error) {
        showToast(t('loadFailed'), 'error');
    } finally {
        hideLoading();
    }
}

function displayCategories(categories) {
    const container = document.getElementById('categoriesList');
    container.innerHTML = '';
    categories.forEach(cat => {
        const div = document.createElement('div');
        div.className = 'grid-item';
        div.innerHTML = `
            <h3>${cat.categoryName}</h3>
            <p><strong>Arabic:</strong> ${cat.categoryNameAr}</p>
            <p>${cat.description}</p>
            <p><strong>Items:</strong> ${cat.menuItemsCount}</p>
            <div class="item-footer">
                <button class="btn btn-info" onclick="showEditCategoryModal(${cat.id})">${t('edit')}</button>
                <button class="btn btn-danger" onclick="showDeleteConfirmModal('Category', ${cat.id}, '${cat.categoryName}')">${t('delete')}</button>
            </div>
        `;
        container.appendChild(div);
    });
}

function showCreateCategoryModal() {
    openModal('createCategoryModal');
}

async function handleCreateCategory(e) {
    e.preventDefault();
    showLoading();

    const data = {
        categoryName: document.getElementById('catName').value,
        categoryNameAr: document.getElementById('catNameAr').value,
        description: document.getElementById('catDescription').value,
        displayOrder: parseInt(document.getElementById('catDisplayOrder').value)
    };

    try {
        const response = await fetch(`${API_BASE_URL}/Menu/categories`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${authToken}`
            },
            body: JSON.stringify(data)
        });
        const result = await response.json();
        if (result.success) {
            showToast(t('createSuccess'), 'success');
            closeModal('createCategoryModal');
            loadCategories();
            document.getElementById('createCategoryForm').reset();
        } else {
            showToast(result.message, 'error');
        }
    } catch (error) {
        showToast(t('createFailed'), 'error');
    } finally {
        hideLoading();
    }
}

async function deleteCategory(id) {
    showLoading();
    try {
        const response = await fetch(`${API_BASE_URL}/Menu/categories/${id}`, {
            method: 'DELETE',
            headers: { 'Authorization': `Bearer ${authToken}` }
        });
        const result = await response.json();
        if (result.success) {
            showToast(t('deleteSuccess'), 'success');
            loadCategories();
        } else {
            showToast(result.message, 'error');
        }
    } catch (error) {
        showToast(t('deleteFailed'), 'error');
    } finally {
        hideLoading();
    }
}

// ===== MENU ITEMS =====
async function loadMenuItems() {
    showLoading();
    try {
        const response = await fetch(`${API_BASE_URL}/Menu/items?page=1&pageSize=100`, {
            headers: { 'Authorization': `Bearer ${authToken}` }
        });
        const result = await response.json();
        if (result.success) {
            displayMenuItems(result.data.items);
        }
    } catch (error) {
        showToast(t('loadFailed'), 'error');
    } finally {
        hideLoading();
    }
}

function displayMenuItems(items) {
    const container = document.getElementById('menuItemsList');
    container.innerHTML = '';
    items.forEach(item => {
        const div = document.createElement('div');
        div.className = 'grid-item';
        const availableClass = item.isAvailable ? 'status-available' : 'status-unavailable';
        const availableText = item.isAvailable ? '✅ Available' : '❌ Unavailable';
        div.innerHTML = `
            ${item.imageUrl ? `<img src="${item.imageUrl}" alt="${item.name}" onerror="this.style.display='none'">` : ''}
            <h3>${item.name}</h3>
            <p><strong>Arabic:</strong> ${item.nameAr}</p>
            <p>${item.description}</p>
            <p><strong>Category:</strong> ${item.categoryName || 'N/A'}</p>
            <p class="price">${item.price.toFixed(2)} EGP</p>
            <p><strong>Prep Time:</strong> ${item.preparationTime} mins</p>
            <p><strong>Status:</strong> <span class="status-badge ${availableClass}">${availableText}</span></p>
            <div class="item-footer">
                <button class="btn btn-info" onclick="showEditMenuItemModal(${item.id})">${t('edit')}</button>
                <button class="btn btn-warning" onclick="toggleAvailability(${item.id})">Toggle</button>
                <button class="btn btn-danger" onclick="showDeleteConfirmModal('Menu Item', ${item.id}, '${item.name.replace(/'/g, "\\'")}')">${t('delete')}</button>
            </div>
        `;
        container.appendChild(div);
    });
}

async function showCreateMenuItemModal() {
    showLoading();
    try {
        const response = await fetch(`${API_BASE_URL}/Menu/categories`, {
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
        showToast(t('loadFailed'), 'error');
    } finally {
        hideLoading();
    }
}

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
        preparationTime: parseInt(document.getElementById('menuItemPrepTime').value)
    };

    try {
        const response = await fetch(`${API_BASE_URL}/Menu/items`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${authToken}`
            },
            body: JSON.stringify(data)
        });
        const result = await response.json();
        if (result.success) {
            showToast(t('createSuccess'), 'success');
            closeModal('createMenuItemModal');
            loadMenuItems();
            document.getElementById('createMenuItemForm').reset();
        } else {
            showToast(result.message, 'error');
        }
    } catch (error) {
        showToast(t('createFailed'), 'error');
    } finally {
        hideLoading();
    }
}

async function toggleAvailability(id) {
    showLoading();
    try {
        const response = await fetch(`${API_BASE_URL}/Menu/items/${id}/toggle-availability`, {
            method: 'PATCH',
            headers: { 'Authorization': `Bearer ${authToken}` }
        });
        const result = await response.json();
        if (result.success) {
            showToast(result.message, 'success');
            loadMenuItems();
        }
    } catch (error) {
        showToast(t('updateFailed'), 'error');
    } finally {
        hideLoading();
    }
}

async function deleteMenuItem(id) {
    showLoading();
    try {
        const response = await fetch(`${API_BASE_URL}/Menu/items/${id}`, {
            method: 'DELETE',
            headers: { 'Authorization': `Bearer ${authToken}` }
        });
        const result = await response.json();
        if (result.success) {
            showToast(t('deleteSuccess'), 'success');
            loadMenuItems();
        } else {
            showToast(result.message, 'error');
        }
    } catch (error) {
        showToast(t('deleteFailed'), 'error');
    } finally {
        hideLoading();
    }
}

// ===== TABLES =====
async function loadTables() {
    showLoading();
    try {
        const response = await fetch(`${API_BASE_URL}/Table`, {
            headers: { 'Authorization': `Bearer ${authToken}` }
        });
        const result = await response.json();
        if (result.success) {
            displayTables(result.data);
        }
    } catch (error) {
        showToast(t('loadFailed'), 'error');
    } finally {
        hideLoading();
    }
}

function displayTables(tables) {
    const container = document.getElementById('tablesList');
    container.innerHTML = '';
    tables.forEach(table => {
        const div = document.createElement('div');
        div.className = 'grid-item';
        div.innerHTML = `
            <h3>Table #${table.tableNumber}</h3>
            <p><strong>Capacity:</strong> ${table.capacity} people</p>
            <p><strong>Section:</strong> ${table.floorSection}</p>
            <p><strong>Status:</strong> <span class="status-badge status-${table.status.toLowerCase()}">${table.status}</span></p>
            ${table.currentOrderNumber ? `<p><strong>Order:</strong> ${table.currentOrderNumber}</p>` : ''}
            <div class="item-footer">
                <button class="btn btn-danger" onclick="showDeleteConfirmModal('Table', ${table.id}, 'Table #${table.tableNumber}')">${t('delete')}</button>
            </div>
        `;
        container.appendChild(div);
    });
}

function showCreateTableModal() {
    openModal('createTableModal');
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
        const response = await fetch(`${API_BASE_URL}/Table`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${authToken}`
            },
            body: JSON.stringify(data)
        });
        const result = await response.json();
        if (result.success) {
            showToast(t('createSuccess'), 'success');
            closeModal('createTableModal');
            loadTables();
            document.getElementById('createTableForm').reset();
        } else {
            showToast(result.message, 'error');
        }
    } catch (error) {
        showToast(t('createFailed'), 'error');
    } finally {
        hideLoading();
    }
}

async function deleteTable(id) {
    showLoading();
    try {
        const response = await fetch(`${API_BASE_URL}/Table/${id}`, {
            method: 'DELETE',
            headers: { 'Authorization': `Bearer ${authToken}` }
        });
        const result = await response.json();
        if (result.success) {
            showToast(t('deleteSuccess'), 'success');
            loadTables();
        } else {
            showToast(result.message, 'error');
        }
    } catch (error) {
        showToast(t('deleteFailed'), 'error');
    } finally {
        hideLoading();
    }
}

// ===== ORDERS =====
async function loadOrders() {
    showLoading();
    try {
        const response = await fetch(`${API_BASE_URL}/Order/active`, {
            headers: { 'Authorization': `Bearer ${authToken}` }
        });
        const result = await response.json();
        if (result.success) {
            displayOrders(result.data);
        }
    } catch (error) {
        showToast(t('loadFailed'), 'error');
    } finally {
        hideLoading();
    }
}

function displayOrders(orders) {
    const container = document.getElementById('ordersList');
    container.innerHTML = '';
    if (orders.length === 0) {
        container.innerHTML = '<p style="color: white; text-align: center; font-size: 18px;">No active orders</p>';
        return;
    }
    orders.forEach(order => {
        const div = document.createElement('div');
        div.className = 'grid-item';
        let itemsHtml = '';
        if (order.items && order.items.length > 0) {
            itemsHtml = '<ul style="margin: 10px 0; padding-left: 20px;">';
            order.items.forEach(item => {
                itemsHtml += `<li>${item.quantity}x ${item.menuItemName} (${item.unitPrice.toFixed(2)} EGP)</li>`;
            });
            itemsHtml += '</ul>';
        }
        div.innerHTML = `
            <h3>Order #${order.orderNumber}</h3>
            <p><strong>Table:</strong> ${order.tableNumber || 'N/A'}</p>
            <p><strong>Waiter:</strong> ${order.userName || 'N/A'}</p>
            <p><strong>Status:</strong> <span class="status-badge status-${order.status.toLowerCase()}">${order.status}</span></p>
            <p><strong>Items (${order.items ? order.items.length : 0}):</strong></p>
            ${itemsHtml}
            <p class="price">Total: ${order.totalAmount.toFixed(2)} EGP</p>
            <p><small>${new Date(order.orderDate).toLocaleString()}</small></p>
            ${order.notes ? `<p><em>Notes: ${order.notes}</em></p>` : ''}
            <div class="item-footer">
                ${order.status === 'Pending' ? `<button class="btn btn-warning" onclick="updateOrderStatus(${order.id}, 'Preparing')">Start Preparing</button>` : ''}
                ${order.status === 'Preparing' ? `<button class="btn btn-warning" onclick="updateOrderStatus(${order.id}, 'Ready')">Mark Ready</button>` : ''}
                ${order.status === 'Ready' ? `<button class="btn btn-success" onclick="updateOrderStatus(${order.id}, 'Served')">Mark Served</button>` : ''}
                <button class="btn btn-danger" onclick="showDeleteConfirmModal('Order', ${order.id}, 'Order #${order.orderNumber}')">Cancel</button>
            </div>
        `;
        container.appendChild(div);
    });
}

let orderItemCount = 0;
let availableMenuItems = [];

async function showCreateOrderModal() {
    showLoading();
    try {
        const [tablesRes, itemsRes] = await Promise.all([
            fetch(`${API_BASE_URL}/Table/available`, {
                headers: { 'Authorization': `Bearer ${authToken}` }
            }),
            fetch(`${API_BASE_URL}/Menu/items?page=1&pageSize=100`, {
                headers: { 'Authorization': `Bearer ${authToken}` }
            })
        ]);
        const tablesResult = await tablesRes.json();
        const itemsResult = await itemsRes.json();
        if (tablesResult.success && itemsResult.success) {
            const tableSelect = document.getElementById('orderTable');
            tableSelect.innerHTML = tablesResult.data.map(t =>
                `<option value="${t.id}">Table #${t.tableNumber} (${t.capacity} seats)</option>`
            ).join('');
            availableMenuItems = itemsResult.data.items.filter(item => item.isAvailable);
            orderItemCount = 0;
            document.getElementById('orderItemsList').innerHTML = '';
            addOrderItem();
            openModal('createOrderModal');
        }
    } catch (error) {
        showToast(t('loadFailed'), 'error');
    } finally {
        hideLoading();
    }
}

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
        `<option value="${item.id}">${item.name} - ${item.price.toFixed(2)} EGP</option>`
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
        const response = await fetch(`${API_BASE_URL}/Order`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${authToken}`
            },
            body: JSON.stringify(data)
        });
        const result = await response.json();
        if (result.success) {
            showToast(t('createSuccess'), 'success');
            closeModal('createOrderModal');
            loadOrders();
            loadTables();
        } else {
            showToast(result.message, 'error');
        }
    } catch (error) {
        showToast(t('createFailed'), 'error');
    } finally {
        hideLoading();
    }
}

async function updateOrderStatus(orderId, newStatus) {
    showLoading();
    try {
        const response = await fetch(`${API_BASE_URL}/Order/${orderId}/status`, {
            method: 'PATCH',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${authToken}`
            },
            body: JSON.stringify({ status: newStatus })
        });
        const result = await response.json();
        if (result.success) {
            showToast(t('updateSuccess'), 'success');
            loadOrders();
            loadTables();
        } else {
            showToast(result.message, 'error');
        }
    } catch (error) {
        showToast(t('updateFailed'), 'error');
    } finally {
        hideLoading();
    }
}

async function cancelOrder(id) {
    showLoading();
    try {
        const response = await fetch(`${API_BASE_URL}/Order/${id}`, {
            method: 'DELETE',
            headers: { 'Authorization': `Bearer ${authToken}` }
        });
        const result = await response.json();
        if (result.success) {
            showToast(t('deleteSuccess'), 'success');
            loadOrders();
            loadTables();
        } else {
            showToast(result.message, 'error');
        }
    } catch (error) {
        showToast(t('deleteFailed'), 'error');
    } finally {
        hideLoading();
    }
}

// ===== SETTINGS =====
async function loadEmailSettings() {
    try {
        const response = await fetch(`${API_BASE_URL}/Settings/email`, {
            headers: { 'Authorization': `Bearer ${authToken}` }
        });
        const result = await response.json();
        if (result.success) {
            const settings = result.data;
            document.getElementById('settingSmtpServer').value = settings.smtpServer || '';
            document.getElementById('settingSmtpPort').value = settings.smtpPort || '';
            document.getElementById('settingFromEmail').value = settings.fromEmail || '';
            document.getElementById('settingPassword').value = settings.password || '';
            document.getElementById('settingEnableSsl').checked = settings.enableSsl;
        }
    } catch (error) {
        console.error('Failed to load email settings:', error);
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
        const response = await fetch(`${API_BASE_URL}/Settings/email`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${authToken}`
            },
            body: JSON.stringify(data)
        });
        const result = await response.json();
        if (result.success) {
            showToast(t('updateSuccess'), 'success');
        } else {
            showToast(result.message, 'error');
        }
    } catch (error) {
        showToast(t('updateFailed'), 'error');
    } finally {
        hideLoading();
    }
}

async function handleForgotPassword(e) {
    e.preventDefault();
    showLoading();

    const email = document.getElementById('forgotEmail').value;
    otpEmail = email;


    console.log('=== FORGOT PASSWORD DEBUG ===');
    console.log('Email entered:', email);
    console.log('API URL:', `${API_BASE_URL}/Auth/forgot-password`);

    try {
        const response = await fetch(`${API_BASE_URL}/Auth/forgot-password`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ email })
        });

        console.log('Response status:', response.status);
        console.log('Response OK:', response.ok);

        // Try to get response text first
        const responseText = await response.text();
        console.log('Raw response:', responseText);

        // Try to parse as JSON
        let result;
        try {
            result = JSON.parse(responseText);
            console.log('Parsed response:', result);
        } catch (parseError) {
            console.error('Failed to parse JSON:', parseError);
            showToast('Invalid response from server', 'error');
            hideLoading();
            return;
        }

        if (result.success) {
            console.log('✅ OTP sent successfully!');
            showToast('OTP sent to your email!', 'success');
            document.getElementById('forgotStep1').style.display = 'none';
            document.getElementById('forgotStep2').style.display = 'block';
            startResendTimer();
        } else {
            console.error('❌ API returned success=false');
            console.error('Error message:', result.message);
            showToast(result.message || 'Failed to send OTP', 'error');
        }
    } catch (error) {
        console.error('=== CAUGHT ERROR ===');
        console.error('Error type:', error.name);
        console.error('Error message:', error.message);
        console.error('Full error:', error);

        showToast('Failed to send OTP. Check console for details.', 'error');
    } finally {
        hideLoading();
        console.log('=== DEBUG END ===');
    }
}

//async function handleForgotPassword(e) {
//    e.preventDefault();
//    showLoading();

//    const email = document.getElementById('forgotEmail').value;

//    try {
//        const response = await fetch(`${API_BASE_URL}/Auth/forgot-password`, {
//            method: 'POST',
//            headers: { 'Content-Type': 'application/json' },
//            body: JSON.stringify({ email })
//        });

//        // ✅ Check if response is OK first
//        if (!response.ok) {
//            throw new Error('Network response was not ok');
//        }

//        const result = await response.json();

//        if (result.success) {
//            showToast('OTP sent to your email!', 'success');
//            document.getElementById('forgotStep1').style.display = 'none';
//            document.getElementById('forgotStep2').style.display = 'block';
//        } else {
//            showToast(result.message || 'Failed to send OTP', 'error');
//        }
//    } catch (error) {
//        console.error('Forgot password error:', error);  // ✅ Log for debugging
//        showToast('Failed to send OTP. Please check your connection.', 'error');
//    } finally {
//        hideLoading();
//    }
//}

async function handleResetPassword(e) {
    e.preventDefault();
    showLoading();

    const email = document.getElementById('forgotEmail').value;
    const otp = document.getElementById('otpCode').value;
    const newPassword = document.getElementById('newPassword').value;
    const confirmPassword = document.getElementById('confirmPassword').value;

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

// ===== CHANGE PASSWORD =====
function showChangePasswordModal() {
    const dropdown = document.getElementById('userDropdown');
    dropdown?.classList.remove('show');
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
        const loginResponse = await fetch(`${API_BASE_URL}/Auth/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                userName: currentUser.userName,
                password: currentPassword
            })
        });
        const loginResult = await loginResponse.json();
        if (loginResult.success) {
            showToast('Password changed successfully!', 'success');
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

// OTP Timer and Resend
let otpEmail = '';
let resendTimer = null;
let resendCountdown = 0;

function startResendTimer() {
    resendCountdown = 60;
    const resendBtn = document.getElementById('resendOtpBtn');

    if (resendTimer) clearInterval(resendTimer);

    resendTimer = setInterval(() => {
        resendCountdown--;
        if (resendBtn) {
            if (resendCountdown > 0) {
                resendBtn.textContent = `Resend OTP (${resendCountdown}s)`;
                resendBtn.disabled = true;
                resendBtn.style.opacity = '0.5';
                resendBtn.style.cursor = 'not-allowed';
                resendBtn.style.color = '#666';
            } else {
                resendBtn.textContent = 'Resend OTP';
                resendBtn.disabled = false;
                resendBtn.style.opacity = '1';
                resendBtn.style.cursor = 'pointer';
                resendBtn.style.color = '#a78bfa';
            }
        }
    }, 1000);

    // Add hover effect
    if (resendBtn) {
        resendBtn.addEventListener('mouseenter', function () {
            if (!this.disabled) {
                this.style.color = '#667eea';
                this.style.textDecoration = 'underline';
            }
        });
        resendBtn.addEventListener('mouseleave', function () {
            if (!this.disabled) {
                this.style.color = '#a78bfa';
                this.style.textDecoration = 'none';
            }
        });
    }
}

async function resendOTP() {
    if (resendCountdown > 0) return;

    showLoading();
    try {
        const response = await fetch(`${API_BASE_URL}/Auth/forgot-password`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email: otpEmail })
        });

        const result = await response.json();

        if (result.success) {
            showToast('New OTP sent!', 'success');
            startResendTimer();
        } else {
            showToast('Failed to resend OTP', 'error');
        }
    } catch (error) {
        showToast('Failed to resend OTP', 'error');
    } finally {
        hideLoading();
    }
}

// ALSO UPDATE YOUR handleForgotPassword FUNCTION:
// Add this line after: const email = document.getElementById('forgotEmail').value;
// otpEmail = email;

// Add this line inside the success block:
// startResendTimer();
// ===== EDIT PROFILE =====
function showEditProfileModal() {
    const dropdown = document.getElementById('userDropdown');
    dropdown?.classList.remove('show');

    document.getElementById('editFullName').value = currentUser.fullName || '';
    document.getElementById('editEmail').value = currentUser.email || '';
    document.getElementById('editUsername').value = currentUser.userName || '';

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
        showToast('Profile updated successfully!', 'success');
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

// ===== EDIT MODALS (DYNAMIC) =====
async function showEditCategoryModal(id) {
    showLoading();
    try {
        const response = await fetch(`${API_BASE_URL}/Menu/categories/${id}`, {
            headers: { 'Authorization': `Bearer ${authToken}` }
        });
        const result = await response.json();
        if (result.success) {
            const cat = result.data;
            const modalHtml = `
                <div id="editCategoryModal" class="modal show" style="display: flex;">
                    <div class="modal-content">
                        <span class="close" onclick="closeModal('editCategoryModal')">&times;</span>
                        <h2>${t('edit')} Category</h2>
                        <form id="editCategoryForm" onsubmit="handleEditCategory(event, ${cat.id}); return false;">
                            <div class="form-group">
                                <label>Category Name</label>
                                <input type="text" id="editCatName" value="${cat.categoryName}" required>
                            </div>
                            <div class="form-group">
                                <label>Category Name (Arabic)</label>
                                <input type="text" id="editCatNameAr" value="${cat.categoryNameAr}" required>
                            </div>
                            <div class="form-group">
                                <label>Description</label>
                                <textarea id="editCatDescription" rows="3">${cat.description || ''}</textarea>
                            </div>
                            <div class="form-group">
                                <label>Display Order</label>
                                <input type="number" id="editCatDisplayOrder" value="${cat.displayOrder}" required>
                            </div>
                            <button type="submit" class="btn btn-primary">${t('update')}</button>
                        </form>
                    </div>
                </div>
            `;
            document.body.insertAdjacentHTML('beforeend', modalHtml);
        }
    } catch (error) {
        showToast(t('loadFailed'), 'error');
    } finally {
        hideLoading();
    }
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
        const response = await fetch(`${API_BASE_URL}/Menu/categories/${id}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${authToken}`
            },
            body: JSON.stringify(data)
        });
        const result = await response.json();
        if (result.success) {
            showToast(t('updateSuccess'), 'success');
            closeModal('editCategoryModal');
            loadCategories();
        } else {
            showToast(result.message, 'error');
        }
    } catch (error) {
        showToast(t('updateFailed'), 'error');
    } finally {
        hideLoading();
    }
}

async function showEditMenuItemModal(id) {
    showLoading();
    try {
        const [categoriesRes, itemRes] = await Promise.all([
            fetch(`${API_BASE_URL}/Menu/categories`, {
                headers: { 'Authorization': `Bearer ${authToken}` }
            }),
            fetch(`${API_BASE_URL}/Menu/items/${id}`, {
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
                        <h2>${t('edit')} Menu Item</h2>
                        <form id="editMenuItemForm" onsubmit="handleEditMenuItem(event, ${item.id}); return false;">
                            <div class="form-group">
                                <label>Category</label>
                                <select id="editItemCategory" required>
                                    ${categoriesOptions}
                                </select>
                            </div>
                            <div class="form-group">
                                <label>Item Name</label>
                                <input type="text" id="editItemName" value="${item.name}" required>
                            </div>
                            <div class="form-group">
                                <label>Item Name (Arabic)</label>
                                <input type="text" id="editItemNameAr" value="${item.nameAr}" required>
                            </div>
                            <div class="form-group">
                                <label>Description</label>
                                <textarea id="editItemDescription" rows="3">${item.description || ''}</textarea>
                            </div>
                            <div class="form-group">
                                <label>Price (EGP)</label>
                                <input type="number" step="0.01" id="editItemPrice" value="${item.price}" required>
                            </div>
                            <div class="form-group">
                                <label>Image URL</label>
                                <input type="text" id="editItemImageUrl" value="${item.imageUrl || ''}" placeholder="https://example.com/image.jpg">
                            </div>
                            <div class="form-group">
                                <label>Preparation Time (minutes)</label>
                                <input type="number" id="editItemPrepTime" value="${item.preparationTime}" required>
                            </div>
                            <button type="submit" class="btn btn-primary">${t('update')}</button>
                        </form>
                    </div>
                </div>
            `;
            document.body.insertAdjacentHTML('beforeend', modalHtml);
        }
    } catch (error) {
        showToast(t('loadFailed'), 'error');
    } finally {
        hideLoading();
    }
}

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
        preparationTime: parseInt(document.getElementById('editItemPrepTime').value)
    };

    try {
        const response = await fetch(`${API_BASE_URL}/Menu/items/${id}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${authToken}`
            },
            body: JSON.stringify(data)
        });
        const result = await response.json();
        if (result.success) {
            showToast(t('updateSuccess'), 'success');
            closeModal('editMenuItemModal');
            loadMenuItems();
        } else {
            showToast(result.message, 'error');
        }
    } catch (error) {
        showToast(t('updateFailed'), 'error');
    } finally {
        hideLoading();
    }
}