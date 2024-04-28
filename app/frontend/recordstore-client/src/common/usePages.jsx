import { useState } from 'react';

const usePages = (initialPage = 1) => {
  const [page, setPage] = useState(initialPage);

  const [totalPages, setTotalPages] = useState(1);

  const nextPage = () => {
    setPage((prevPage) => prevPage + 1);
  };

  const prevPage = () => {
    setPage((prevPage) => prevPage - 1);
  };

  const goToPage = (page) => {
    if (page > 0 && page <= totalPages) {
      setPage(page);
    }
  };

  return {
    page,
    setPage,
    totalPages,
    setTotalPages,
    nextPage,
    prevPage,
    goToPage,
  };
};

export default usePages;
