import React, { useState, useEffect } from 'react';
import {
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalFooter,
  ModalBody,
  ModalCloseButton,
  FormControl,
  FormLabel,
  Input,
  Textarea,
  Button,
  useToast,
  Select,
  Box,
  Image,
} from '@chakra-ui/react';

import {
  AutoComplete,
  AutoCompleteInput,
  AutoCompleteList,
  AutoCompleteItem,
  AutoCompleteTag,
} from '@choc-ui/chakra-autocomplete';

// should use useDebounce in this, couldn't get it to work
// import { useDebounce } from '@uidotdev/usehooks';
import productService from '../hooks/ProductService';
import genreService from '../hooks/GenreService';
import artistService from '../hooks/ArtistService';
import formatService from '../hooks/FormatService';

const EditProductForm = ({ isOpen, onClose, product, onSubmit }) => {
  const [formData, setFormData] = useState({
    // title: product ? product.title : '',
    description: product ? product.description : '',
    price: product ? product.price : '',
    formatName: product ? (product.format || {}).name : '', // should be format.name (not format
    releaseDate: product ? product.releaseDate : '',
    quantity: product ? product.quantity : '',
    location: product ? product.location : '',
    restockLevel: product ? product.restockLevel : '',
    imageUrl: product ? product.imageUrl : '',
  });

  const [id, setId] = useState(product ? product.id : '');

  //   const [searchArtistTerm, setSearchArtistTerm] = useState('');
  //   const [artists, setArtists] = useState([]);
  //   const [searchGenreTerm, setSearchGenreTerm] = useState('');
  //   const [genres, setGenres] = useState([]);
  const [searchFormatTerm, setSearchFormatTerm] = useState('');
  const [formats, setFormats] = useState([]);

  useEffect(() => {
    console.log(formData);
  }, [formData]);

  //   useEffect(() => {
  //     const fetchArtists = async () => {
  //       try {
  //         const response = await artistService.searchArtists(searchArtistTerm);
  //         setArtists(response);
  //       } catch (error) {
  //         console.error('Error fetching artists:', error);
  //       }
  //     };
  //     if (searchArtistTerm) {
  //       fetchArtists();
  //     }
  //   }, [searchArtistTerm]);

  //   useEffect(() => {
  //     const fetchGenres = async () => {
  //       try {
  //         const response = await genreService.getGenres(searchGenreTerm);
  //         setGenres(response);
  //       } catch (error) {
  //         console.error('Error fetching genres:', error);
  //       }
  //     };
  //     if (searchGenreTerm) {
  //       fetchGenres();
  //     }
  //   }, [searchGenreTerm]);

  useEffect(() => {
    const fetchFormats = async () => {
      try {
        const response = await formatService.searchFormats(searchFormatTerm);
        setFormats(response);
      } catch (error) {
        console.error('Error fetching formats:', error);
      }
    };
    if (searchFormatTerm) {
      fetchFormats();
    }
  }, [searchFormatTerm]);

  useEffect(() => {
    if (product) {
      setFormData({
        // title: product ? product.title : '',
        description: product ? product.description : '',
        price: product ? product.price : '',
        formatName: product ? (product.format || {}).name : '', // should be format.name (not format
        quantity: product ? product.quantity : '',
        location: product ? product.location : '',
        restockLevel: product ? product.restockLevel : '',
        imageUrl: product ? product.imageUrl : '',
      });

      setId(product.id);
    }
    setSearchFormatTerm(product ? (product.format || {}).name : '');
  }, [product]);

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  //   const handleArtistChange = (selectedArtists) => {
  //     setFormData({ ...formData, artists: selectedArtists });
  //   };

  //   const handleGenreChange = (vals) => {
  //     setFormData({ ...formData, genres: vals });
  //   };

  const handleFormatChange = (name) => {
    setFormData({ ...formData, formatName: name });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    await onSubmit(id, formData);
  };
  return (
    <Modal isOpen={isOpen} onClose={onClose} size='xl'>
      <ModalOverlay />
      <ModalContent>
        <ModalHeader>Редагувати продукт</ModalHeader>
        <ModalCloseButton />
        <form onSubmit={handleSubmit}>
          <ModalBody>
            <Image
              src={formData.imageUrl}
              fallbackSrc='https://via.placeholder.com/150'
              alt={formData.title}
              boxSize='150px'
              mb={4}
            />
            <FormControl id='imageUrl' mb={4}>
              <FormLabel>Посилання на зображення</FormLabel>
              <Input
                name='imageUrl'
                value={formData.imageUrl}
                onChange={handleChange}
              />
            </FormControl>
            <FormControl id='description' mb={4}>
              <FormLabel>Опис</FormLabel>
              <Textarea
                name='description'
                value={formData.description}
                onChange={handleChange}
              />
            </FormControl>
            <FormControl id='price' mb={4}>
              <FormLabel>Ціна (грн)</FormLabel>
              <Input
                name='price'
                type='number'
                value={formData.price}
                onChange={handleChange}
              />
            </FormControl>
            <FormControl id='formats' mb={4}>
              <FormLabel>Формат</FormLabel>
              <AutoComplete openOnFocus mb={4}>
                <AutoCompleteInput
                  placeholder='Формат'
                  onChange={(e) => {
                    setSearchFormatTerm(e.target.value);
                  }}
                  value={searchFormatTerm}
                  mb={4}
                />
                <AutoCompleteList>
                  {formats.map((option, index) => (
                    <AutoCompleteItem
                      key={index}
                      onClick={() => {
                        setSearchFormatTerm(option.name);
                        handleFormatChange(option.name);
                      }}
                      value={option.name}
                    >
                      {option.name}
                    </AutoCompleteItem>
                  ))}
                </AutoCompleteList>
              </AutoComplete>
            </FormControl>
            <FormControl id='quantity' mb={4}>
              <FormLabel>Кількість</FormLabel>
              <Input
                name='quantity'
                type='number'
                value={formData.quantity}
                onChange={handleChange}
              />
            </FormControl>
            <FormControl id='location' mb={4}>
              <FormLabel>Локація</FormLabel>
              <Input
                name='location'
                value={formData.location}
                onChange={handleChange}
              />
            </FormControl>
            <FormControl id='restockLevel' mb={4}>
              <FormLabel>Рівень поповнення</FormLabel>
              <Input
                name='restockLevel'
                type='number'
                value={formData.restockLevel}
                onChange={handleChange}
              />
            </FormControl>
          </ModalBody>
          <ModalFooter>
            <Button colorScheme='blue' mr={3} type='submit'>
              Оновити
            </Button>
            <Button variant='ghost' onClick={onClose}>
              Скасувати
            </Button>
          </ModalFooter>
        </form>
      </ModalContent>
    </Modal>
  );
};

export default EditProductForm;
