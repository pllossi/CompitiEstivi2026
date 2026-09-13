import { ArticleService } from "../service/ArticleService.js";

const articleService = new ArticleService();

let currentArticles = [];
let selectedArticle = null;
let isEditing = false;

const listEl = document.getElementById('article-list');
const reloadBtn = document.getElementById('reload-articles-button');
const saveBtn = document.getElementById('save-article-button');
const deleteBtn = document.getElementById('delete-article-button');
const editBtn = document.getElementById('edit-article-button');
const cancelBtn = document.getElementById('cancel-article-button');
const titleBox = document.getElementById('article-title-box');
const contentBox = document.getElementById('article-content');
const searchTitleInput = document.getElementById('search-title');
const searchContentInput = document.getElementById('search-content');
const searchContentBtn = document.getElementById('search-content-button');
const searchDateInput = document.getElementById('search-date');
const searchDateBtn = document.getElementById('search-date-button');
const searchStartInput = document.getElementById('search-start-date');
const searchEndInput = document.getElementById('search-end-date');
const searchDateRangeBtn = document.getElementById('search-date-range-button');
const newArticleBtn = document.getElementById('new-article-button');
const deselectArticleBtn = document.getElementById('deselect-article-button');
const messageAlert = document.getElementById('message-alert');


function showMessage(message, type = 'success') {
  // Clear previous
  messageAlert.innerHTML = '';
  const div = document.createElement('div');
  div.className = `alert alert-${type} alert-dismissible fade show`;
  div.role = 'alert';
  div.innerHTML = `
    ${message}
    <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
  `;
  // Add click listener to close button
  const btn = div.querySelector('.btn-close');
  btn.addEventListener('click', () => {
    div.remove();
  });
  messageAlert.appendChild(div);
  // Auto remove after 5 seconds
  setTimeout(() => {
    if (div.parentNode) {
      div.remove();
    }
  }, 5000);
}

function renderList(articles) {
  listEl.innerHTML = '';
  const items = Array.isArray(articles) ? articles.filter(a => a) : (articles ? [articles] : []);
  if (!items || items.length === 0) {
    const li = document.createElement('li');
    li.className = 'list-group-item';
    li.textContent = 'Nessun articolo';
    listEl.appendChild(li);
    return;
  }

  items.forEach(a => {
    const li = document.createElement('li');
    li.className = 'list-group-item';
    li.dataset.id = a.Id;
    li.style.cursor = 'pointer';
    const articleEl = document.createElement('article');
    articleEl.id = `article-${a.Id}`;

    const h3 = document.createElement('h3');
    h3.className = 'card-title';
    h3.textContent = a.Title;

    const datePara = document.createElement('p');
    datePara.className = 'card-text';
    datePara.textContent = a.formattedDate();

    articleEl.appendChild(h3);
    articleEl.appendChild(datePara);
    li.appendChild(articleEl);

    li.addEventListener('click', () => {
      if (selectedArticle && selectedArticle.Id === a.Id) {
        deselectArticle();
      } else {
        selectArticle(a);
      }
    });
    listEl.appendChild(li);
  });
}

function selectArticle(article) {
  selectedArticle = article;
  titleBox.value = article.Title ?? '';
  contentBox.value = article.Content ?? '';
  isEditing = false;
  setEditingMode(false);
  const prev = listEl.querySelector('.selected');
  if (prev) prev.classList.remove('selected');
  const currentLi = listEl.querySelector(`[data-id="${article.Id}"]`);
  if (currentLi) currentLi.classList.add('selected');
  if (deselectArticleBtn) deselectArticleBtn.disabled = false;
}

function deselectArticle() {
  selectedArticle = null;
  const prev = listEl.querySelector('.selected');
  if (prev) prev.classList.remove('selected');
  titleBox.value = '';
  contentBox.value = '';
  setEditingMode(false);
  if (deselectArticleBtn) deselectArticleBtn.disabled = true;
}

function setEditingMode(edit) {
  isEditing = edit;
  titleBox.disabled = !edit;
  contentBox.disabled = !edit;
  saveBtn.disabled = !edit;
  cancelBtn.disabled = !edit;
  editBtn.disabled = edit || !selectedArticle;
  deleteBtn.disabled = !selectedArticle;
  if (deselectArticleBtn) deselectArticleBtn.disabled = !selectedArticle;
}

async function loadArticles() {
  try {
    const articles = await articleService.getAll();
    currentArticles = articles;
    renderList(currentArticles);
    setEditingMode(false);
    showMessage('Articoli caricati con successo', 'success');
  } catch (err) {
    showMessage(err.message || err, 'danger');
  }
}

reloadBtn.addEventListener('click', loadArticles);

saveBtn.addEventListener('click', async () => {
  try {
    const art = {
      Id: selectedArticle?.Id ?? undefined,
      Title: titleBox.value,
      Content: contentBox.value,
      Author: selectedArticle?.Author ?? '',
      Timestamp: selectedArticle?.Timestamp ?? undefined,
    };
    const saved = await articleService.save(art);
    await loadArticles();
    if (saved) {
      selectArticle(saved);
    } else {
      deselectArticle();
    }
    setEditingMode(false);
    showMessage('Articolo salvato', 'success');
  } catch (err) {
    showMessage(err.message || err, 'danger');
  }
});

deleteBtn.addEventListener('click', async () => {
  if (!selectedArticle) return alert('Nessun articolo selezionato');
  if (!confirm('Sei sicuro di voler eliminare l\'articolo?')) return;
  try {
    await articleService.delete(selectedArticle.Id);
    selectedArticle = null;
    titleBox.value = '';
    contentBox.value = '';
    await loadArticles();
    showMessage('Articolo eliminato', 'success');
  } catch (err) {
    showMessage(err.message || err, 'danger');
  }
});

editBtn.addEventListener('click', () => {
  if (!selectedArticle) return alert('Nessun articolo selezionato');
  setEditingMode(true);
});

cancelBtn.addEventListener('click', () => {
  if (selectedArticle) selectArticle(selectedArticle);
  else {
    titleBox.value = '';
    contentBox.value = '';
    setEditingMode(false);
  }
});

searchContentBtn.addEventListener('click', async () => {
  const q = searchContentInput.value.trim();
  searchTitleInput.value = '';
  if (!q) return loadArticles();
  const results = await articleService.searchByContent(q);
  renderList(results);
  setEditingMode(false);
  showMessage('Ricerca per contenuto effettuata', 'info');
});

searchDateBtn.addEventListener('click', async () => {
  const d = searchDateInput.value;
  searchTitleInput.value = '';
  if (!d) return loadArticles();
  const results = await articleService.searchByDate(d);
  renderList(results);
  setEditingMode(false);
  showMessage('Ricerca per data effettuata', 'info');
});

searchDateRangeBtn.addEventListener('click', async () => {
  const s = searchStartInput.value;
  const e = searchEndInput.value;
  searchTitleInput.value = '';
  if (!s || !e) return loadArticles();
  if (s > e) return alert('La data di inizio deve essere precedente a quella di fine');
  const results = await articleService.searchByDateRange(s, e);
  renderList(results);
  setEditingMode(false);
  showMessage('Ricerca per intervallo di date effettuata', 'info');
});

newArticleBtn.addEventListener('click', () => {
  selectedArticle = null;
  titleBox.value = '';
  contentBox.value = '';
  setEditingMode(true);
});

deselectArticleBtn.addEventListener('click', deselectArticle);

// Real-time search by title or author
searchTitleInput.addEventListener('input', () => {
  const query = searchTitleInput.value.trim();
  const filtered = currentArticles.filter(a => {
    const titleMatch = (a.Title ?? '').toLowerCase().includes(query.toLowerCase());
    const authorMatch = (a.Author ?? '').toLowerCase().includes(query.toLowerCase());
    return titleMatch || authorMatch;
  });
  renderList(filtered);
  // deselect any selected article when filtering changes
  if (selectedArticle) {
    // if selected article not in filtered list, deselect
    if (!filtered.some(a => a.Id === selectedArticle.Id)) {
      deselectArticle();
    }
  }
});

// Initialize
loadArticles();