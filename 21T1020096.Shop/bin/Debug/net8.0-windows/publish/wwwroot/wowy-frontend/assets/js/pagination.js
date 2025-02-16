const PaginationModule = {
    // Bind this cho các methods
    renderPagination: function (currentPage, totalPages, container) {
        if (!container) return;

        container.innerHTML = '';
        const ul = document.createElement('ul');
        ul.className = 'pagination justify-content-center';

        // Bind this cho createPageItem
        const createPageItem = (pageNum, isActive = false, isDisabled = false) => {
            const li = document.createElement('li');
            li.className = `page-item${isActive ? ' active' : ''}${isDisabled ? ' disabled' : ''}`;

            const a = document.createElement('a');
            a.className = 'page-link';
            a.href = 'javascript:;';

            if (!isActive && !isDisabled) {
                // Bind this cho onclick
                a.onclick = () => PaginationModule.doSearch(pageNum);
            }

            a.textContent = pageNum;
            li.appendChild(a);
            return li;
        };

        const createEllipsis = () => {
            const li = document.createElement('li');
            li.className = 'page-item disabled';
            const span = document.createElement('span');
            span.className = 'page-link';
            span.textContent = '...';
            li.appendChild(span);
            return li;
        };

        if (totalPages <= 10) {
            for (let i = 1; i <= totalPages; i++) {
                ul.appendChild(createPageItem(i, i === currentPage));
            }
        } else {
            ul.appendChild(createPageItem(1, currentPage === 1));

            if (currentPage <= 3) {
                ul.appendChild(createPageItem(2, currentPage === 2));
            }

            if (currentPage > 3) {
                ul.appendChild(createEllipsis());
            }

            for (let i = Math.max(3, currentPage - 1); i <= Math.min(totalPages - 2, currentPage + 1); i++) {
                ul.appendChild(createPageItem(i, i === currentPage));
            }

            if (currentPage < totalPages - 2) {
                ul.appendChild(createEllipsis());
            }

            if (currentPage >= totalPages - 2) {
                ul.appendChild(createPageItem(totalPages - 1, currentPage === totalPages - 1));
            }

            ul.appendChild(createPageItem(totalPages, currentPage === totalPages));
        }

        container.appendChild(ul);
    },

    doSearch: function (page) {
        var searchCondition = $("#frmSearchInput").serialize() + '&page=' + page;
        var action = $("#frmSearchInput").prop("action");
        var method = $("#frmSearchInput").prop("method");
        $.ajax({
            url: action,
            type: method,
            data: searchCondition,
            success: (data) => {
                var tempDiv = document.createElement('div');
                tempDiv.innerHTML = data;

                var content = tempDiv.querySelector('#mainContent').innerHTML;
                $("#searchResult").html(content);

                const currentPage = parseInt(tempDiv.querySelector('[data-current-page]').dataset.currentPage || '1');
                const totalPages = parseInt(tempDiv.querySelector('[data-total-pages]').dataset.totalPages || '1');

                const paginationContainer = document.querySelector('#paginationContainer');
                if (paginationContainer) {
                    // Gọi trực tiếp thông qua PaginationModule
                    PaginationModule.renderPagination(
                        currentPage,
                        totalPages,
                        paginationContainer
                    );
                }
            },
            error: (xhr, status, error) => {
                console.error('Search error:', error);
            }
        });
    },

    init: function () {
        if ($("#frmSearchInput").length && $("#searchResult").length) {
            const currentPage = parseInt($('[data-current-page]').data('currentPage') || '1');
            const totalPages = parseInt($('[data-total-pages]').data('totalPages') || '1');

            const paginationContainer = document.querySelector('#paginationContainer');
            if (paginationContainer) {
                // Gọi trực tiếp thông qua PaginationModule
                PaginationModule.renderPagination(
                    currentPage,
                    totalPages,
                    paginationContainer
                );
            }

            $("#frmSearchInput").submit((e) => {
                e.preventDefault();
                PaginationModule.doSearch(1);
            });

            // Khởi tạo search ban đầu
            PaginationModule.doSearch(currentPage);
        }
    }
};